# Rappelz Launcher

A Windows launcher for a Rappelz game client, built with **C#**, **Windows Forms**, and **.NET 10**. It displays server-hosted content through **WebView2**, stores launcher settings, checks for updates, applies client patches in version order, and can update its own executable using a separate updater process.

> This is an independent launcher project. Rappelz and related trademarks belong to their respective owners. This repository does not include the game client or grant rights to distribute game assets.

## Features

- **Windows Forms interface** with account and password fields, an AutoLogin option, a Start Game button, and download progress.
- **WebView2 browser area** that loads the URL configured in the launcher settings. The browser content is rendered as supplied by the website; the launcher does not need to inject layout scripts or parse news content.
- **Encrypted configuration** using AES-256-GCM for the entire `RappelzLauncher.ini` file (in the newer configuration implementation).
- **Launcher self-update** when a newer launcher version is listed in the server manifest.
- **Sequential client patches** installed in increasing version order, overwriting files in the installation directory.
- **Download feedback** showing the current archive name and progress percentage.
- **Separate updater executable** so the running launcher can be closed and replaced.

## Technology stack

| Component | Technology |
| --- | --- |
| Application | C# / .NET 10 (`net10.0-windows`) |
| UI | Windows Forms (WinForms) |
| Embedded browser | Microsoft Edge WebView2 |
| Configuration encryption | AES-256-GCM (`System.Security.Cryptography`) |
| Networking | `HttpClient` / HTTP(S) |
| Update manifest | XML (`System.Xml.Linq`) |
| Archives | ZIP / .NET compression APIs |
| Build system | Visual Studio / MSBuild / .NET CLI |
| Target platform | Windows x64 |

## Solution structure

```text
RappelzLauncher.sln
src/
  RappelzLauncher/
    Core/
      ArchiveInstaller.cs
      LauncherSettings.cs
      PatchInstaller.cs
      UpdateDownloader.cs
      UpdateManifest.cs
    MainForm.cs
    MainForm.Designer.cs
    SelfUpdateLauncher.cs
    RappelzLauncher.csproj
  RappelzLauncher.Updater/
    RappelzLauncher.Updater.csproj
```

The solution contains **two projects**: the graphical launcher and the auxiliary updater. Core classes are part of the launcher project, not a separate class-library project.

## Configuration and encryption

The launcher uses `RappelzLauncher.ini` in its installation directory. Its *decrypted* structure is:

```ini
#########################
[Login]
account=
password=
useAutoLogin=false
[SframeParams]
param=
launcherUrl=
#########################
```

- `account` / `password`: optional login credentials.
- `useAutoLogin`: `true` or `false`.
- `param`: arguments used to start the game client.
- `launcherUrl`: base URL used for the website and the update manifest.

In the **encrypted configuration implementation**, the whole INI—not individual values—is protected by **AES-256-GCM**. The file is stored as binary data consisting of a random **12-byte nonce**, a **16-byte authentication tag**, and the encrypted payload. No INI sections, keys, or values are intentionally left as readable text on disk. `Load()` decrypts and parses the content in memory; `Save()` updates the supported settings and encrypts the complete content again.

The encrypted-only format **rejects legacy plaintext INI files**. If the INI is missing, the launcher can create the default configuration directly in encrypted form. An invalid authentication tag or incompatible key must produce an error rather than silently overwriting an existing configuration.

**Security limitations:** AES-GCM protects confidentiality and detects ciphertext modification, but a shared encryption key embedded in a client-distributed executable may be extracted by someone who controls that executable. Do not put server-side secrets, administrator credentials, or private signing keys in the client INI. Login passwords saved by the launcher can be recovered by a sufficiently capable local attacker. Use HTTPS for server endpoints when possible.

> **Important:** Before publishing, confirm that the checked-in `LauncherSettings.cs` actually contains the final encrypted-only implementation. An earlier project ZIP used plaintext INI reads/writes and is not compatible with the description above.

## Update manifest

The launcher reads `updates.xml` from the configured `launcherUrl` base address. Update ZIP files are downloaded from the `updates/` subdirectory under that address.

```xml
<?xml version="1.0" encoding="utf-8"?>
<updates>
    <launcherupdate>
        <version>2026.05.20.0000</version>
        <file>20260520.zip</file>
    </launcherupdate>
    <patches>
        <patch>
            <version>1.1.0</version>
            <file>v110.zip</file>
        </patch>
        <patch>
            <version>1.2.0</version>
            <file>v120.zip</file>
        </patch>
        <patch>
            <version>1.3.0</version>
            <file>v130.zip</file>
        </patch>
    </patches>
</updates>
```

Example server layout:

```text
https://your-server.example/
  updates.xml
  updates/
    20260520.zip
    v110.zip
    v120.zip
    v130.zip
```

### Launcher self-update

1. Compare the current launcher executable version with `<launcherupdate><version>`.
2. If the server version is newer, download the specified ZIP from `updates/`.
3. Start `RappelzLauncher.Updater.exe` as an auxiliary process and close the launcher.
4. The updater waits for the launcher to exit, extracts the update into the **same installation directory**, overwrites files supplied by the archive, and starts the updated launcher.
5. A launcher update archive may include a replacement `RappelzLauncher.ini`. It is **not automatically preserved** against an explicitly supplied update file.

Launcher versions use four numeric components in the intended format **`YYYY.MM.DD.HHMM`**, for example `2026.05.20.1435`. The project may use `FileVersion` as the source of the running executable version; keep the manifest and compiled file version consistent. In .NET project files, version components are numeric, so leading zeroes may not be retained in their displayed form.

### Client patches

1. Read the installed client version from the text file named `version` next to `RappelzLauncher.exe`.
2. Select all `<patch>` entries whose version is newer than the installed version.
3. Download **one patch at a time in ascending version order**.
4. Extract and overwrite that patch's contents **in the launcher installation directory**.
5. Update the local `version` file **only after successful installation of that patch**.
6. Continue until all applicable patches are applied; otherwise stop and report the failure.

Example: if the local `version` contains `1.1.0`, the launcher installs `v120.zip`, then `v130.zip`, in that order. The `<patches>` element is a list and can contain as many `<patch>` entries as required.

**Packaging rule:** ZIP paths are relative to the installation directory. Test patch and launcher archives in a separate installation before publishing. The local `version` file is distinct from the executable's `FileVersion`.

## Build and deployment

Requirements for development:

- Windows and Visual Studio with the .NET desktop development workload, or the .NET 10 SDK.
- Access to the WebView2 NuGet dependency.

Build the solution:

```powershell
dotnet build RappelzLauncher.sln -c Release
```

Publish the launcher:

```powershell
dotnet publish .\src\RappelzLauncher\RappelzLauncher.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

The solution's MSBuild configuration is intended to build/publish the updater alongside the launcher. Check the actual output directory after building; `dotnet build` and `dotnet publish` produce different output layouts.

The intended **minimal distribution** is:

```text
RappelzLauncher.exe
RappelzLauncher.Updater.exe
RappelzLauncher.ini
```

The `version` file is additionally required for client patch tracking. Since this is a **framework-dependent** deployment, clients must have the compatible **.NET 10 Windows Desktop Runtime** and **Microsoft Edge WebView2 Runtime** installed. Other dependencies may be generated during development builds; distribute from the verified publish output, not the entire `bin` directory.

Do not distribute local credentials, developer build artifacts, temporary downloads, or encryption keys separately.

## Security and operational notes

- Treat manifest files and ZIP packages as untrusted remote input. Validate archive paths to prevent path traversal.
- Deliver manifest and patch files over trusted HTTPS where possible.
- AES-GCM configuration encryption **does not authenticate update ZIPs**. For production deployments, add package integrity checks and preferably **cryptographic signatures** verified against a public key embedded in the launcher. Keep the signing private key outside client distributions.
- Ensure that a patch is fully downloaded and extracted before advancing the local `version` file.
- Avoid modifying the game installation while the game is running.
- Back up a test installation before exercising self-updates or patches that overwrite files.
- The WebView2 area renders remote content. Do not enable privileged host objects or expose internal application APIs to arbitrary webpages.

## License

This project is intended to be distributed under the **MIT License**. Include a separate `LICENSE` file containing the full MIT license text and the correct copyright holder/year before publishing the repository.

The MIT License applies only to code and assets that the repository owner has the rights to license. Third-party libraries, Rappelz trademarks, game files, and externally sourced artwork retain their own terms and licenses.

See: https://opensource.org/license/mit

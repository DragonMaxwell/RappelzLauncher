# RappelzLauncher

Windows Forms launcher targeting .NET 10, using native WinForms panels and Microsoft Edge WebView2 for the center website panel.

## Projects

- `src/RappelzLauncher`: launcher, INI settings, updater download logic, UI and WebView2.
- `src/RappelzLauncher.Updater`: separate executable that replaces the launcher after the previous process exits.

Open `RappelzLauncher.sln` in Visual Studio with .NET 10 support.

## Installation and publication

The application is published as a framework-dependent single file. It does not embed the .NET runtime into the executables. **.NET 10 Windows Desktop Runtime (x64)** and **Microsoft Edge WebView2 Runtime** must already be installed on each client.

Run this command from the solution directory:

`dotnet publish .\src\RappelzLauncher\RappelzLauncher.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true`

Use the generated `publish` directory, not the regular `bin\Release` build directory. The intended distribution files are:

- `RappelzLauncher.exe`
- `RappelzLauncher.Updater.exe`
- `RappelzLauncher.ini`

The `RappelzLauncher.ini` file contains no default server URL. It is created when missing, and edits to existing INI values do not replace the entire file. ZIP-based launcher updates can replace the INI if the package contains it.

The patch system additionally uses a local file named `version` in the installation directory. That file is part of the game's version state and is not a framework dependency or a publication output. The WebView2 profile is stored under `%LOCALAPPDATA%\RappelzLauncher\WebView2`.

## Remote updates

`launcherUrl` is the website root. The updater reads `<launcherUrl>/updates.xml` and downloads archives from `<launcherUrl>/updates/<file>.zip`.

Launcher updates compare the four-part `AssemblyVersion` (`YYYY.MM.DD.HHMM`) with `launcherupdate/version`. The helper process waits for the old process to exit, extracts the update and launches the replacement executable.

Patch updates read the local `version` file and apply every newer `patches/patch` entry in ascending order, extracting each package into the directory of `RappelzLauncher.exe` and then advancing the local version.

## Designer

The form design is declared as standard WinForms controls in `MainForm.Designer.cs`. All runtime event binding and web navigation are in `MainForm.cs`, and opening the Designer does not create the INI.

## Build verification

Source layout and XML files were inspected in this environment. A Windows Visual Studio build and runtime GUI test were not possible here because the .NET SDK and Windows Forms Designer are unavailable.

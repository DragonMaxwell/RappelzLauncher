# Custom Launcher for Rappelz

This is an independent project designed to simplify the creation of a custom launcher for the game.

Feel free to use it as you like! If this project helped you, please don't forget to **cite the source** and **leave a star (⭐)** on the repository to support its development.

## 🛠️ Technologies Used

* **.NET 10**
* **WebView2**
* **WinForms**

## 📦 Prerequisites / Required Libraries

To run and use the launcher properly, make sure you have the following runtimes installed:

* [.NET 10 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-10.0.15-windows-x64-installer)
* [WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/consumer?form=MA13LH&cs=3508739050)

## ⚙️ Configuration (`RappelzLauncher.ini`)

The launcher behavior is managed via the `.ini` configuration file. Below is an example of the structure used:

```ini
[Login]
account=account
password=password
useAutoLogin=false
testMode=false

[SframeParams]
param=RappelzCmdLauncher.exe SFrame.exe /auth_ip:127.0.0.1 /auth_port:4615 /locale:ASCII /country:CN /cash /commercial_shop /use_nprotect:0
launcherUrl=https://your_server_address
```

### 📋 Parameter Description

#### `[Login]` Section

* `account`: Username/account required for the auto-login feature.
* `password`: Password required for the auto-login feature.
* `useAutoLogin`: Enables or disables automatic account login (`true` or `false`).
* `testMode`: Enables test mode (`true` or `false`). Useful for forcing specific configurations, suppressing errors, or decrypting the `RappelzLauncher.ini` file *(the `.ini` file is only decrypted while the launcher is open)*.

#### `[SframeParams]` Section

* `param`: Command-line startup parameters passed directly to `SFrame.exe`.
* `launcherUrl`: URL of your website/panel (the starting file must be `index.html`).

## 📄 License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).

```text
MIT License

Copyright (c) 2026

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
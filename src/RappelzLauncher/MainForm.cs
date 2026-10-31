using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Web.WebView2.Core;
using RappelzLauncher.Core;
using System.Drawing.Imaging;

namespace RappelzLauncher;

public partial class MainForm : Form
{
    private readonly string _installationDirectory = AppContext.BaseDirectory;
    private readonly CancellationTokenSource _shutdown = new();
    private readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromMinutes(30) };
    private readonly LauncherSettings _settings;
    private int _downloadPercentage;
    private bool _restarting;
    private bool _updating;

    public MainForm()
    {
        InitializeComponent();

        _startButton.Parent!.MouseMove += (_, e) =>
        {
            _startButton.Parent.Cursor = !_startButton.Enabled && _startButton.Bounds.Contains(e.Location) ? Cursors.No : Cursors.Default;
        };

        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        {
            _settings = new LauncherSettings();
            return;
        }

        _settings = LauncherSettings.Load(Path.Combine(_installationDirectory, "RappelzLauncher.ini"));
        Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        _startButton.Click += StartGame;
        _progressTrack.Resize += (_, _) => RenderProgress();
        _accountInput.Text = _settings.Account;
        _passwordInput.Text = _settings.Password;
        _autoLoginInput.Checked = _settings.UseAutoLogin;

        Shown += OnShown;
        FormClosing += OnClosing;
        FormClosed += OnClosed;

        SetButtonEnabled(_startButton, false);
    }

    private async void OnShown(object? sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_settings.LauncherUrl))
            {
                ShowProgress("Server URL is not configured", null);
                return;
            }

            Uri serverUri = _settings.GetServerUri();
            Task browserTask = OpenWebsiteAsync(serverUri);
            Task updateTask = CheckUpdatesAsync(serverUri);
            await Task.WhenAll(browserTask, updateTask);
        }
        catch (OperationCanceledException) when (_shutdown.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            ShowError("Launcher Error", exception);
        }
    }

    private async Task OpenWebsiteAsync(Uri serverUri)
    {
        try
        {
            string dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RappelzLauncher", "WebView2");
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(userDataFolder: dataDirectory);
            await _webView.EnsureCoreWebView2Async(environment);

            _webView.CoreWebView2.NavigationCompleted += (_, arguments) =>
            {
                if (_shutdown.IsCancellationRequested || IsDisposed)
                {
                    return;
                }

                if (!arguments.IsSuccess)
                {
                    ShowError("Website Error", new IOException($"Failed to load {serverUri}: {arguments.WebErrorStatus}"));
                }
            };

            _webView.CoreWebView2.Navigate(serverUri.AbsoluteUri);
        }
        catch (OperationCanceledException) when (_shutdown.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            ShowError("Website Error", exception);
        }
    }

    private async Task CheckUpdatesAsync(Uri serverUri)
    {
        if (_updating)
        {
            return;
        }

        _updating = true;
        IProgress<DownloadProgress> progress = new Progress<DownloadProgress>(value =>
        {
            if (!IsDisposed && !_shutdown.IsCancellationRequested)
            {
                ShowProgress(value.FileName, value.Percentage);
            }
        });

        try
        {
            string manifestUrl = new Uri(new Uri(UpdateDownloader.EnsureTrailingSlash(serverUri)), "updates.xml").AbsoluteUri;
            string xml = await _httpClient.GetStringAsync(manifestUrl, _shutdown.Token);
            UpdateManifest manifest = UpdateManifest.Parse(xml);
            Version currentVersion = Version.Parse(FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion ?? throw new InvalidOperationException("Launcher Version not found."));
            UpdateDownloader downloader = new(_httpClient);

            if (manifest.Launcher.Version > currentVersion)
            {
                string archive = await downloader.DownloadAsync(serverUri, manifest.Launcher.FileName, progress, _shutdown.Token);

                try
                {
                    SelfUpdateLauncher.Start(archive, _installationDirectory, manifest.Launcher.Version);
                    _restarting = true;
                    Close();
                }
                catch
                {
                    ArchiveInstaller.DeleteDownload(archive);
                    throw;
                }

                return;
            }

            PatchInstaller patchInstaller = new(downloader);
            await patchInstaller.InstallAsync(serverUri, manifest.Patches, _installationDirectory, progress, _shutdown.Token);
            
            ShowProgress("Client is up to date", 100);
            SetButtonEnabled(_startButton, true);
        }
        catch (OperationCanceledException) when (_shutdown.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            if (!_shutdown.IsCancellationRequested && !IsDisposed)
            {
                ShowProgress("Update failed", 0);
                ShowError("Update Error", exception);
            }
        }
        finally
        {
            _updating = false;
        }
    }

    private void StartGame(object? sender, EventArgs e)
    {
        try
        {
            SaveSettings();
            string executable = Path.Combine(_installationDirectory, "RappelzCmdLauncher.exe");

            if (!File.Exists(executable))
            {
                throw new FileNotFoundException("RappelzCmdLauncher.exe was not found.", executable);
            }

            string arguments = _settings.Parameters.Trim();
            if (_autoLoginInput.Checked)
            {
                arguments += $" /imbclogin /account:{QuoteArgument(_accountInput.Text)} /password:{QuoteArgument(_passwordInput.Text)}";
            }

            ProcessStartInfo startInfo = new(executable)
            {
                WorkingDirectory = _installationDirectory,
                Arguments = arguments.Trim(),
                UseShellExecute = true
            };

            using Process? gameProcess = Process.Start(startInfo);
            if (gameProcess is null)
            {
                throw new InvalidOperationException("Unable to start the game.");
            }

            Close();
        }
        catch (Exception exception)
        {
            ShowError("Game Launch Error", exception);
        }
    }

    private static string QuoteArgument(string value)
    {
        return "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
    }

    private void OnClosing(object? sender, FormClosingEventArgs e)
    {
        try
        {
            SaveSettings();
        }
        catch (Exception exception)
        {
            e.Cancel = true;
            ShowError("Configuration Error", exception);
            return;
        }

        _shutdown.Cancel();
    }

    private void OnClosed(object? sender, FormClosedEventArgs e)
    {
        _httpClient.Dispose();
        _shutdown.Dispose();
    }

    private void SaveSettings()
    {
        _settings.Account = _accountInput.Text;
        _settings.Password = _passwordInput.Text;
        _settings.UseAutoLogin = _autoLoginInput.Checked;
        _settings.Save(Path.Combine(_installationDirectory, "RappelzLauncher.ini"));
    }

    private void ShowProgress(string fileName, int? percentage)
    {
        _downloadFileLabel.Text = fileName;
        _downloadPercentLabel.Text = percentage.HasValue ? $"{percentage.Value}%" : string.Empty;
        _downloadPercentage = Math.Clamp(percentage ?? 0, 0, 100);
        RenderProgress();
    }

    private void RenderProgress()
    {
        _progressFill.Width = (int)Math.Round(_progressTrack.ClientSize.Width * _downloadPercentage / 100d);
        _progressFill.Height = _progressTrack.ClientSize.Height;
    }

    private void ShowError(string title, Exception exception)
    {
        if (!IsDisposed && !_shutdown.IsCancellationRequested)
        {
            MessageBox.Show(this, exception.GetBaseException().Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SetButtonEnabled(Button button, bool enabled)
    {
        button.FlatAppearance.MouseOverBackColor = Color.Transparent;

        if (enabled)
        {
            button.Cursor = Cursors.Hand;

            if (button.Tag is Image original)
            {
                Image? disabled = button.BackgroundImage;
                button.BackgroundImage = original;
                button.Tag = null;
                disabled?.Dispose();
            }
        }
        else if (button.Tag is not Image && button.BackgroundImage is Image image)
        {
            Bitmap grayscale = new Bitmap(image.Width, image.Height);

            using (Graphics graphics = Graphics.FromImage(grayscale))
            using (ImageAttributes attributes = new ImageAttributes())
            {
                ColorMatrix matrix = new ColorMatrix(new float[][]
                {
                    new float[] { 0.21f, 0.21f, 0.21f, 0, 0 },
                    new float[] { 0.72f, 0.72f, 0.72f, 0, 0 },
                    new float[] { 0.07f, 0.07f, 0.07f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                });

                attributes.SetColorMatrix(matrix);

                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }

            button.Tag = image;
            button.BackgroundImage = grayscale;
            button.Cursor = Cursors.No;
        }
        

        button.Enabled = enabled;
    }
}

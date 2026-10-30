using System.Diagnostics;

namespace RappelzLauncher;

internal static class SelfUpdateLauncher
{
    public static void Start(string archivePath, string installationDirectory, Version expectedVersion)
    {
        string updaterName = "RappelzLauncher.Updater.exe";
        string updaterSource = Path.Combine(installationDirectory, updaterName);

        if (!File.Exists(updaterSource))
        {
            throw new FileNotFoundException("The launcher updater executable was not found.", updaterSource);
        }

        string temporaryDirectory = Path.Combine(Path.GetTempPath(), "RappelzLauncher", "Updater", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(temporaryDirectory);
        string updaterPath = Path.Combine(temporaryDirectory, updaterName);
        File.Copy(updaterSource, updaterPath);

        string launcherPath = Environment.ProcessPath ?? throw new InvalidOperationException("Launcher executable path is unavailable.");
        ProcessStartInfo startInfo = new(updaterPath)
        {
            UseShellExecute = true,
            WorkingDirectory = temporaryDirectory
        };

        startInfo.ArgumentList.Add(Environment.ProcessId.ToString());
        startInfo.ArgumentList.Add(archivePath);
        startInfo.ArgumentList.Add(installationDirectory);
        startInfo.ArgumentList.Add(Path.GetFileName(launcherPath));
        startInfo.ArgumentList.Add(expectedVersion.ToString());

        using Process? updaterProcess = Process.Start(startInfo);
        if (updaterProcess is null)
        {
            throw new InvalidOperationException("Could not start the launcher updater.");
        }
    }
}

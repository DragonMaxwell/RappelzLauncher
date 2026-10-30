using System.Diagnostics;
using RappelzLauncher.Core;

namespace RappelzLauncher.Updater;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        try
        {
            if (args.Length != 5 || !int.TryParse(args[0], out int processId))
            {
                throw new ArgumentException("Invalid updater arguments.");
            }

            string archivePath = args[1];
            string installationDirectory = Path.GetFullPath(args[2]);
            string launcherName = args[3];
            Version expectedVersion = Version.Parse(args[4]);
            string launcherPath = ArchiveInstaller.GetSafePath(installationDirectory, launcherName);

            WaitForLauncher(processId);
            string stagingDirectory = Path.Combine(Path.GetTempPath(), "RappelzLauncher", Guid.NewGuid().ToString("N"));
            string backupDirectory = Path.Combine(Path.GetTempPath(), "RappelzLauncher", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(stagingDirectory);
            Directory.CreateDirectory(backupDirectory);

            try
            {
                ArchiveInstaller.ExtractToStaging(archivePath, stagingDirectory);
                string replacement = ArchiveInstaller.GetSafePath(stagingDirectory, launcherName);

                if (!File.Exists(replacement))
                {
                    throw new InvalidDataException($"The update archive does not contain {launcherName}.");
                }

                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(replacement);
                Version replacementVersion = new(fileInfo.FileMajorPart, fileInfo.FileMinorPart, fileInfo.FileBuildPart, fileInfo.FilePrivatePart);

                if (replacementVersion != expectedVersion)
                {
                    throw new InvalidDataException($"The version of {launcherName} does not match the update manifest.");
                }

                ArchiveInstaller.InstallStagedFiles(stagingDirectory, installationDirectory, backupDirectory, launcherName);
                ArchiveInstaller.DeleteDownload(archivePath);

                ProcessStartInfo startInfo = new(launcherPath)
                {
                    WorkingDirectory = installationDirectory,
                    UseShellExecute = true
                };

                using Process? restartedLauncher = Process.Start(startInfo);
                if (restartedLauncher is null)
                {
                    throw new InvalidOperationException("Could not restart the updated launcher.");
                }
            }
            finally
            {
                if (Directory.Exists(stagingDirectory))
                {
                    Directory.Delete(stagingDirectory, true);
                }

                if (Directory.Exists(backupDirectory))
                {
                    Directory.Delete(backupDirectory, true);
                }
            }

            return 0;
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Launcher update failed: {exception.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 1;
        }
    }

    private static void WaitForLauncher(int processId)
    {
        try
        {
            using Process process = Process.GetProcessById(processId);
            if (!process.WaitForExit(120000))
            {
                throw new TimeoutException("The current launcher did not exit within two minutes.");
            }
        }
        catch (ArgumentException)
        {
        }
    }
}

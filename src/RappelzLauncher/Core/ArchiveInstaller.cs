using System.IO.Compression;

namespace RappelzLauncher.Core;

public static class ArchiveInstaller
{
    public static void Install(string archivePath, string installationDirectory)
    {
        string root = Path.GetFullPath(installationDirectory);
        string stagingDirectory = Path.Combine(Path.GetTempPath(), "RappelzLauncher", Guid.NewGuid().ToString("N"));
        string backupDirectory = Path.Combine(Path.GetTempPath(), "RappelzLauncher", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(stagingDirectory);
        Directory.CreateDirectory(backupDirectory);

        try
        {
            ExtractToStaging(archivePath, stagingDirectory);
            InstallStagedFiles(stagingDirectory, root, backupDirectory);
        }
        finally
        {
            DeleteDirectoryIfExists(stagingDirectory);
            DeleteDirectoryIfExists(backupDirectory);
        }
    }

    public static void ExtractToStaging(string archivePath, string stagingDirectory)
    {
        string root = Path.GetFullPath(stagingDirectory);
        Directory.CreateDirectory(root);

        using ZipArchive archive = ZipFile.OpenRead(archivePath);
        if (!archive.Entries.Any(entry => !entry.FullName.EndsWith('/')))
        {
            throw new InvalidDataException("The update archive is empty.");
        }

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            string relativePath = entry.FullName.Replace('/', Path.DirectorySeparatorChar);
            string destination = GetSafePath(root, relativePath);
            bool isDirectory = entry.FullName.EndsWith('/');
            int unixFileType = (entry.ExternalAttributes >> 16) & 0xF000;

            if (unixFileType == 0xA000)
            {
                throw new InvalidDataException($"Archive contains a symbolic link: {entry.FullName}");
            }

            if (isDirectory)
            {
                Directory.CreateDirectory(destination);
                continue;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            entry.ExtractToFile(destination, true);
        }
    }

    public static void InstallStagedFiles(string stagingDirectory, string installationDirectory, string backupDirectory, string? executableToReplace = null)
    {
        string stagingRoot = Path.GetFullPath(stagingDirectory);
        string installRoot = Path.GetFullPath(installationDirectory);
        List<(string Destination, string? Backup)> processed = new();

        try
        {
            foreach (string source in Directory.EnumerateFiles(stagingRoot, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(stagingRoot, source);
                string destination = GetSafePath(installRoot, relativePath);
                EnsureNoReparsePoints(installRoot, destination);
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

                string? backup = null;
                if (File.Exists(destination))
                {
                    backup = GetSafePath(backupDirectory, relativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(backup)!);
                    File.Copy(destination, backup, true);
                }

                processed.Add((destination, backup));
                if (relativePath.Equals(executableToReplace, StringComparison.OrdinalIgnoreCase) && File.Exists(destination))
                {
                    File.Delete(destination);
                }

                File.Copy(source, destination, true);
            }
        }
        catch
        {
            foreach ((string destination, string? backup) in processed.AsEnumerable().Reverse())
            {
                if (backup is null)
                {
                    File.Delete(destination);
                }
                else
                {
                    File.Copy(backup, destination, true);
                }
            }

            throw;
        }
    }

    public static string GetSafePath(string directory, string relativePath)
    {
        if (Path.IsPathRooted(relativePath) || relativePath.Contains(':'))
        {
            throw new InvalidDataException($"Unsafe archive path: {relativePath}");
        }

        string root = Path.GetFullPath(directory);
        string result = Path.GetFullPath(Path.Combine(root, relativePath));
        string prefix = root.EndsWith(Path.DirectorySeparatorChar) ? root : root + Path.DirectorySeparatorChar;

        if (!result.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException($"Unsafe archive path: {relativePath}");
        }

        return result;
    }

    public static void DeleteDownload(string archivePath)
    {
        string? directory = Path.GetDirectoryName(archivePath);
        if (directory is not null)
        {
            DeleteDirectoryIfExists(directory);
        }
    }

    private static void EnsureNoReparsePoints(string root, string destination)
    {
        string current = Path.GetFullPath(root);
        string relativePath = Path.GetRelativePath(root, destination);

        foreach (string segment in relativePath.Split(Path.DirectorySeparatorChar))
        {
            current = Path.Combine(current, segment);

            if ((File.Exists(current) || Directory.Exists(current)) && (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
            {
                throw new IOException($"Update destination cannot be a symbolic link: {current}");
            }
        }
    }

    private static void DeleteDirectoryIfExists(string directory)
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
    }
}

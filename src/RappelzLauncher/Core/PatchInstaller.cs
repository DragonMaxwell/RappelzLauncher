namespace RappelzLauncher.Core;

public sealed class PatchInstaller(UpdateDownloader downloader)
{
    public async Task InstallAsync(Uri serverUri, IReadOnlyList<UpdatePackage> patches, string installationDirectory, IProgress<DownloadProgress> progress, CancellationToken cancellationToken)
    {
        string versionFile = Path.Combine(installationDirectory, "version");

        if (!File.Exists(versionFile))
        {
            throw new FileNotFoundException("Local version file was not found.", versionFile);
        }

        Version installedVersion = Version.Parse((await File.ReadAllTextAsync(versionFile, cancellationToken)).Trim());

        foreach (UpdatePackage patch in patches.OrderBy(item => item.Version))
        {
            if (patch.Version <= installedVersion)
            {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();
            string archive = string.Empty;

            try
            {
                archive = await downloader.DownloadAsync(serverUri, patch.FileName, progress, cancellationToken);

                await Task.Run(() => ArchiveInstaller.Install(archive, installationDirectory), cancellationToken);
                string versionTemp = $"{versionFile}.{Guid.NewGuid():N}.tmp";

                try
                {
                    await File.WriteAllTextAsync(versionTemp, patch.Version.ToString(), cancellationToken);
                    File.Move(versionTemp, versionFile, true);
                }
                finally
                {
                    if (File.Exists(versionTemp))
                    {
                        File.Delete(versionTemp);
                    }
                }

                installedVersion = patch.Version;
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Failed to install {patch.FileName}\n\n{exception.Message}", "Failed to install", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            finally
            {
                ArchiveInstaller.DeleteDownload(archive);
            }
        }
    }
}

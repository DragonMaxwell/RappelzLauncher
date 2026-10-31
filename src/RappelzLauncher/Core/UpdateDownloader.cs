using System.Buffers;

namespace RappelzLauncher.Core;

public sealed record DownloadProgress(string FileName, int? Percentage);

public sealed class UpdateDownloader(HttpClient httpClient)
{
    public async Task<string> DownloadAsync(Uri serverUri, string fileName, IProgress<DownloadProgress> progress, CancellationToken cancellationToken)
    {
        string directory = Path.Combine(Path.GetTempPath(), "RappelzLauncher", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string destination = Path.Combine(directory, fileName);
        Uri downloadUri = new(new Uri(EnsureTrailingSlash(serverUri)), $"updates/{Uri.EscapeDataString(fileName)}");

        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync(downloadUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();
            long? length = response.Content.Headers.ContentLength;
            long received = 0;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(81920);

            try
            {
                await using Stream source = await response.Content.ReadAsStreamAsync(cancellationToken);
                await using FileStream target = new(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None, buffer.Length, true);
                progress.Report(new DownloadProgress(fileName, length == 0 ? 100 : length.HasValue ? 0 : null));

                int read;
                while ((read = await source.ReadAsync(buffer.AsMemory(), cancellationToken)) > 0)
                {
                    await target.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                    received += read;
                    int? percent = length > 0 ? (int)Math.Clamp(received * 100.0 / length.Value, 0, 100) : null;
                    progress.Report(new DownloadProgress(fileName, percent));
                }

                if (length.HasValue && received != length.Value)
                {
                    throw new IOException($"Incomplete download of {fileName}: {received} of {length.Value} bytes.");
                }

                await target.FlushAsync(cancellationToken);
                progress.Report(new DownloadProgress(fileName, 100));
                return destination;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        catch
        {
            Directory.Delete(directory, true);
            throw;
        }
    }

    public static string EnsureTrailingSlash(Uri uri)
    {
        string url = uri.AbsoluteUri;
        return url.EndsWith('/') ? url : url + "/";
    }
}

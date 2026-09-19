using Lieutenant.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lieutenant.API
{
    public static class InternetAPI
    {
        public static HttpClient client = new();
        public record DownloadProgressReport(double Percentage, long BytesDownloaded, long? TotalBytes);
        private static readonly HttpClient SharedClient = new HttpClient();

        public static async Task DownloadAsync(
            string url,
            string destinationPath,
            IProgress<DownloadProgressReport> progress,
            System.Threading.CancellationToken cancellationToken = default)
        {
            // Force the file streaming off the UI thread completely
            await Task.Run(async () =>
            {
                using var response = await SharedClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                long? totalBytes = response.Content.Headers.ContentLength;

                using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                using var fileStream = new FileStream(
                    destinationPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 65536,
                    useAsync: true);

                var buffer = new byte[65536]; // 64 KB buffer
                long totalBytesRead = 0;
                int bytesRead;

                // Track last update timestamp in milliseconds
                long lastReportTime = Environment.TickCount64;

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                    totalBytesRead += bytesRead;

                    long currentTime = Environment.TickCount64;

                    // ONLY report progress if at least 1000ms (1 second) has passed!
                    if (currentTime - lastReportTime >= 1000)
                    {
                        lastReportTime = currentTime;

                        double percentage = totalBytes.HasValue
                            ? (double)totalBytesRead / totalBytes.Value * 100
                            : 0;

                        progress?.Report(new DownloadProgressReport(percentage, totalBytesRead, totalBytes));
                    }
                }

                // Guaranteed final report at 100% when finished
                progress?.Report(new DownloadProgressReport(100, totalBytesRead, totalBytes));
            }, cancellationToken);
        }
    }
}

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WikimediaProcessor.Services.Downloaders
{
    public class PageViewsDownloader : IPageViewsDownloader
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<PageViewsDownloader> logger;

        public PageViewsDownloader(HttpClient httpClient, ILogger<PageViewsDownloader> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<FileInfo> DownloadPageViews(DateTime date, int hour)
        {
            var fileName = $"pageviews-{date:yyyyMMdd}-{hour:00}0000.gz";
            var url = $"{date.Year}/{date:yyyy-MM}/{fileName}";
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            logger.LogInformation($"File obtained from {response.RequestMessage?.RequestUri}");

            var fileInfo = new FileInfo(fileName);
            await using var ms = await response.Content.ReadAsStreamAsync();
            using var fs = File.Create(fileInfo.FullName);
            ms.Seek(0, SeekOrigin.Begin);
            await ms.CopyToAsync(fs);

            logger.LogInformation($"File saved as [{fileInfo.Name}].");
            return fileInfo;
        }
    }
}

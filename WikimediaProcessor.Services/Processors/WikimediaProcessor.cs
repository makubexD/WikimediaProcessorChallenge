using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WikimediaProcessor.Services.Downloaders;
using WikimediaProcessor.Services.Readers;

namespace WikimediaProcessor.Services.Processors
{
    public class WikimediaProcessor : IWikimediaProcessor
    {
        private readonly IPageViewsDownloader pageViewsDownloader;
        private readonly IPageViewsReader pageViewsReader;
        private readonly ILogger<WikimediaProcessor> logger;

        public WikimediaProcessor(IPageViewsDownloader pageViewsDownloader, IPageViewsReader pageViewsReader, ILogger<WikimediaProcessor> logger)
        {
            this.pageViewsDownloader = pageViewsDownloader;
            this.pageViewsReader = pageViewsReader;
            this.logger = logger;
        }

        public async Task RunAsync(string startYear, string startMonth, string endYear, string endMonth)
        {
            var startMonthValue = string.IsNullOrEmpty(startMonth) ? 1 : Convert.ToInt32(startMonth);
            var endMonthValue = string.IsNullOrEmpty(endMonth) ? 12 : Convert.ToInt32(endMonth);

            logger.LogInformation($"Getting data from {startYear}-{startMonthValue:00} to {endYear}-{endMonthValue:00}.");

            var startDate = new DateTime(Convert.ToInt32(startYear), startMonthValue, 1);
            var endYearParsed = Convert.ToInt32(endYear);
            var endDate = new DateTime(endYearParsed, endMonthValue, DateTime.DaysInMonth(endYearParsed, endMonthValue));
            var currentDateTime = startDate;

            while (currentDateTime <= endDate)
            {
                foreach (var hour in Enumerable.Range(1, 23))
                {
                    FileInfo fileInfo = null;
                    try
                    {
                        fileInfo = await pageViewsDownloader.DownloadPageViews(currentDateTime, hour);
                        await pageViewsReader.ReadContentsAndSaveRecords(fileInfo, currentDateTime);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Error while processing the page views for {currentDateTime:yyyy-MM-dd}, {hour:00} hours: {ex.Message}");
                    }
                    finally
                    {
                        if (fileInfo != null)
                            fileInfo.Delete();
                    }
                }
                currentDateTime = currentDateTime.AddDays(1);
            }
            logger.LogInformation("Process has finished getting data from the Wikimedia API.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WikimediaProcessor.Services.Downloaders;
using WikimediaProcessor.Services.Dto;
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
            //const int MaxHoursInDay = 23; //TODO:  Improve
            const int MaxHoursInDay = 4; // Sample
            var startMonthValue = string.IsNullOrEmpty(startMonth) ? 1 : Convert.ToInt32(startMonth);
            var endMonthValue = string.IsNullOrEmpty(endMonth) ? 12 : Convert.ToInt32(endMonth);

            logger.LogInformation($"Getting data from {startYear}-{startMonthValue:00} to {endYear}-{endMonthValue:00}.");

            var startDate = new DateTime(Convert.ToInt32(startYear), startMonthValue, 1);
            var endYearParsed = Convert.ToInt32(endYear);
            var endDate = new DateTime(endYearParsed, endMonthValue, DateTime.DaysInMonth(endYearParsed, endMonthValue));
            //var endDate = startDate.AddDays(1);
            var currentDateTime = startDate;


            while (currentDateTime <= endDate)            
            {

                var reportLanguageDomainCount = new List<LanguageDomainCount>();
                var reportLanguagePageCount = new List<LanguagePageCount>();

                foreach (var hour in Enumerable.Range(1, MaxHoursInDay))
                {
                    FileInfo fileInfo = null;
                    var reportLanguageDomainCountByHour = new List<LanguageDomainCount>();
                    var reportLanguagePageCountByHour = new List<LanguagePageCount>();

                    try
                    {
                        fileInfo = await pageViewsDownloader.DownloadPageViews(currentDateTime, hour);
                        //await pageViewsReader.ReadContentsAndSaveRecords(fileInfo, currentDateTime);
                        //pageViewsReader.ReadContentsAndSaveRecordsByBulk(fileInfo, currentDateTime);                        
                        (reportLanguageDomainCountByHour, reportLanguagePageCountByHour) = pageViewsReader.Save(pageViewsReader.ReadContents(fileInfo, currentDateTime));
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

                    reportLanguageDomainCount.AddRange(reportLanguageDomainCountByHour);
                    reportLanguagePageCount.AddRange(reportLanguagePageCountByHour);
                }

                await pageViewsReader.SaveLanguageDomainDataPerDay(currentDateTime, endDate, reportLanguageDomainCount);
                await pageViewsReader.SaveLanguagePageDataPerDay(currentDateTime, endDate, reportLanguagePageCount);

                currentDateTime = currentDateTime.AddDays(1);
            }
            logger.LogInformation("Process has finished getting data from the Wikimedia API.");
        }
    }
}

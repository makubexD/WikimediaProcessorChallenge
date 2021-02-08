using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using WikimediaProcessor.Data.KeylessEntities;
using WikimediaProcessor.Data.Repositories;
using WikimediaProcessor.Services.Dto;

namespace WikimediaProcessor.Services.Reporters
{
    public class CsvReporter : ICsvReporter
    {
        private readonly IReportRepository reportRepository;
        private readonly ILogger<CsvReporter> logger;

        public CsvReporter(IReportRepository reportRepository, ILogger<CsvReporter> logger)
        {
            this.reportRepository = reportRepository;
            this.logger = logger;
        }

        public void WriteLanguageDomainReport()
        {
            logger.LogInformation("Writing language domain report.");
            var reportFile = new FileInfo("Language_Domain_count.csv");
            var records = reportRepository.GetLanguageDomainRecords();
            CsvHelper<LanguageDomainReport>.WriteToCsvFile(reportFile.FullName, records, logger);
            logger.LogInformation($"Report was generated in this path: {reportFile.FullName}");
        }        

        public void WriteLanguagePageReport()
        {
            logger.LogInformation("Writing language page report.");
            var reportFile = new FileInfo("Language_Page_count.csv");
            var records = reportRepository.GetLanguagePageRecords();
            CsvHelper<LanguagePageReport>.WriteToCsvFile(reportFile.FullName, records, logger);
            logger.LogInformation($"Report was generated in this path: {reportFile.FullName}");
        }

        public void WriteLanguageDomainReportFromFile(string startYear, string startMonth, string endYear, string endMonth)
        {
            logger.LogInformation("Writing language domain report from files");
            var reportFile = new FileInfo("Language_Domain_count.csv");
            var records = GetRecordByDomain(startYear, startMonth, endYear, endMonth, "LanguageDomain");
            CsvHelper<LanguageDomainCount>.WriteToCsvFile(reportFile.FullName, records, logger);

        }

        public void WriteLanguagePageReportFromFile(string startYear, string startMonth, string endYear, string endMonth)
        {
            logger.LogInformation("Writing language page report  from files.");
            var reportFile = new FileInfo("Language_Page_count.csv");
            var records = GetRecordByPage(startYear, startMonth, endYear, endMonth, "LanguagePage");
            CsvHelper<LanguagePageCount>.WriteToCsvFile(reportFile.FullName, records, logger);
        }

        private List<LanguageDomainCount> GetRecordByDomain(string startYear, string startMonth, string endYear, string endMonth, string prefix)
        {
            var startMonthValue = string.IsNullOrEmpty(startMonth) ? 1 : Convert.ToInt32(startMonth);
            var endMonthValue = string.IsNullOrEmpty(endMonth) ? 12 : Convert.ToInt32(endMonth);

            var startDate = new DateTime(Convert.ToInt32(startYear), startMonthValue, 1);
            var endYearParsed = Convert.ToInt32(endYear);
            var endDate = new DateTime(endYearParsed, endMonthValue, DateTime.DaysInMonth(endYearParsed, endMonthValue));

            var currentDateTime = startDate;
            var monthsQty = MonthDifference(currentDateTime, endDate);

            var reportLanguageDomainCount = new List<LanguageDomainCount>();            

            foreach (var yearMonth in monthsQty)
            {
                var folderName = $"{yearMonth}_{prefix}";
                var files = new DirectoryInfo(folderName).GetFiles();

                foreach (var file in files)
                {
                    using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true))
                    {
                        using TextReader reader = new StreamReader(stream);
                        string line = string.Empty;
                        while ((line = reader.ReadLine()) != null)
                        {
                            try
                            {
                                var parts = line.Split("\t");

                                if (parts.Length != 4)
                                {
                                    continue;
                                }

                                reportLanguageDomainCount.Add(new LanguageDomainCount
                                {
                                    Period = parts.ElementAt(0),
                                    Language = parts.ElementAt(1),
                                    Domain = parts.ElementAt(2),
                                    ViewCount = int.Parse(parts.ElementAt(3))
                                }); ;

                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                        }
                    }

                }
            }

            var result = reportLanguageDomainCount
                    .GroupBy(dt => new { dt.Period, dt.Language, dt.Domain })
                    .Select(d => new LanguageDomainCount()
                    {
                        Period = d.Key.Period,
                        Language = d.Key.Language,
                        Domain = d.Key.Domain,
                        ViewCount = d.Sum(X => X.ViewCount)
                    }).ToList();

            return result;

        }

        private List<LanguagePageCount> GetRecordByPage(string startYear, string startMonth, string endYear, string endMonth, string prefix)
        {
            var startMonthValue = string.IsNullOrEmpty(startMonth) ? 1 : Convert.ToInt32(startMonth);
            var endMonthValue = string.IsNullOrEmpty(endMonth) ? 12 : Convert.ToInt32(endMonth);

            var startDate = new DateTime(Convert.ToInt32(startYear), startMonthValue, 1);
            var endYearParsed = Convert.ToInt32(endYear);
            var endDate = new DateTime(endYearParsed, endMonthValue, DateTime.DaysInMonth(endYearParsed, endMonthValue));

            var currentDateTime = startDate;
            var monthsQty = MonthDifference(currentDateTime, endDate);

            var reportLanguagePageCount = new List<LanguagePageCount>();

            foreach (var yearMonth in monthsQty)
            {
                var folderName = $"{yearMonth}_{prefix}";
                var files = new DirectoryInfo(folderName).GetFiles();

                foreach (var file in files)
                {
                    using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true))
                    {
                        using TextReader reader = new StreamReader(stream);
                        string line = string.Empty;
                        while ((line = reader.ReadLine()) != null)
                        {
                            try
                            {
                                var parts = line.Split("\t");

                                if (parts.Length != 4)
                                {
                                    continue;
                                }

                                reportLanguagePageCount.Add(new LanguagePageCount
                                {
                                    Period = parts.ElementAt(0),
                                    Language = parts.ElementAt(1),
                                    Page = parts.ElementAt(2),
                                    ViewCount = int.Parse(parts.ElementAt(3))
                                }); ;

                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                        }
                    }

                }
            }

            var result = reportLanguagePageCount
                    .GroupBy(dt => new { dt.Period, dt.Language, dt.Page })
                    .Select(d => new LanguagePageCount()
                    {
                        Period = d.Key.Period,
                        Language = d.Key.Language,
                        Page = d.Key.Page,
                        ViewCount = d.Sum(X => X.ViewCount)
                    }).ToList();

            return result;

        }

        public IEnumerable<string> MonthDifference(DateTime start, DateTime end)
        {            
            end = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));

            return Enumerable.Range(0, int.MaxValue)
                                 .Select(e => start.AddMonths(e))
                                 .TakeWhile(e => e <= end)
                                 .Select(e => e.ToString("yyyyMM"));
        }

        

    }
}

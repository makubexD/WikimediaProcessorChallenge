using System.IO;
using Microsoft.Extensions.Logging;
using WikimediaProcessor.Data.KeylessEntities;
using WikimediaProcessor.Data.Repositories;

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
    }
}

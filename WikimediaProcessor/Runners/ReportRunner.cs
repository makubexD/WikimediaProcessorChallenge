using Microsoft.Extensions.Configuration;
using System;
using WikimediaProcessor.Services.Reporters;

namespace WikimediaProcessor.Runners
{
    public class ReportRunner : IReportRunner
    {
        private readonly IConfiguration configuration;
        private readonly ICsvReporter csvReporter;

        public ReportRunner(ICsvReporter csvReporter, IConfiguration configuration)
        {
            this.csvReporter = csvReporter;
            this.configuration = configuration;
        }

        public void Run()
        {
            csvReporter.WriteLanguageDomainReport();
            csvReporter.WriteLanguagePageReport();
        }

        public void RunFromFile()
        {
            var startYear = configuration["AppSettings:StartYear"];
            var endYear = configuration["AppSettings:EndYear"];
            var startMonth = configuration["AppSettings:StartMonth"];
            var endMonth = configuration["AppSettings:EndMonth"];

            if (string.IsNullOrEmpty(startYear))
                throw new ArgumentNullException(nameof(startYear));
            if (string.IsNullOrEmpty(endYear))
                throw new ArgumentNullException(nameof(endYear));

            csvReporter.WriteLanguageDomainReportFromFile(startYear, startMonth, endYear, endMonth);
            csvReporter.WriteLanguagePageReportFromFile(startYear, startMonth, endYear, endMonth);
        }
    }
}

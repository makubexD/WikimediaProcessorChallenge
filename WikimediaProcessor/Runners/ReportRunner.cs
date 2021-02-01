using WikimediaProcessor.Services.Reporters;

namespace WikimediaProcessor.Runners
{
    public class ReportRunner : IReportRunner
    {
        private readonly ICsvReporter csvReporter;

        public ReportRunner(ICsvReporter csvReporter)
        {
            this.csvReporter = csvReporter;
        }

        public void Run()
        {
            csvReporter.WriteLanguageDomainReport();
            csvReporter.WriteLanguagePageReport();
        }
    }
}

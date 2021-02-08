namespace WikimediaProcessor.Services.Reporters
{
    public interface ICsvReporter
    {
        void WriteLanguageDomainReport();
        void WriteLanguagePageReport();
        void WriteLanguageDomainReportFromFile(string startYear, string startMonth, string endYear, string endMonth);
        void WriteLanguagePageReportFromFile(string startYear, string startMonth, string endYear, string endMonth);
    }
}
namespace WikimediaProcessor.Services.Reporters
{
    public interface ICsvReporter
    {
        void WriteLanguageDomainReport();
        void WriteLanguagePageReport();
    }
}
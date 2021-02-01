namespace WikimediaProcessor.Services.Readers
{
    public interface IDomainCodeReader
    {
        (string, string) GetDomainAndLanguage(string domainCode);
    }
}
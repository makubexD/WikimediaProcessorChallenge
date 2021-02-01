using System.Linq;
using WikimediaProcessor.Data.KeylessEntities;

namespace WikimediaProcessor.Data.Repositories
{
    public interface IReportRepository
    {
        IQueryable<LanguageDomainReport> GetLanguageDomainRecords();
        IQueryable<LanguagePageReport> GetLanguagePageRecords();
    }
}
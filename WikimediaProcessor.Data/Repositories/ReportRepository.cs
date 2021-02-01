using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WikimediaProcessor.Data.KeylessEntities;

namespace WikimediaProcessor.Data.Repositories
{
    public class ReportRepository : IReportRepository, IDisposable
    {
        private readonly WikimediaContext context;

        public ReportRepository(IDbContextFactory<WikimediaContext> contextFactory)
        {
            context = contextFactory.CreateDbContext();
        }

        public void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
            }
        }

        public IQueryable<LanguageDomainReport> GetLanguageDomainRecords()
        {
            return context.LanguageDomainReports
                .FromSqlRaw("EXECUTE spLanguageDomainReport");
        }

        public IQueryable<LanguagePageReport> GetLanguagePageRecords()
        {
            return context.LanguagePageReports
                .FromSqlRaw("EXECUTE spLanguagePageReport");
        }
    }
}

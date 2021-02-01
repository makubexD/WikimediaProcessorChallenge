using Microsoft.EntityFrameworkCore;
using WikimediaProcessor.Data.Entities;
using WikimediaProcessor.Data.KeylessEntities;

namespace WikimediaProcessor.Data
{
    public class WikimediaContext : DbContext
    {
        public WikimediaContext(DbContextOptions<WikimediaContext> options)
            : base(options)
        {
        }

        public DbSet<Page> Pages { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<LanguageDomainReport> LanguageDomainReports { get; set; }
        public DbSet<LanguagePageReport> LanguagePageReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Page>().Property(d => d.Created).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Activity>().Property(d => d.Created).HasDefaultValueSql("GETDATE()");

            // TODO: this was added because of this issue: https://github.com/dotnet/efcore/issues/18116
            // Check in the future if this is still needed for keyless entities (query types)
            modelBuilder.Entity<LanguageDomainReport>().HasNoKey().ToView(null);
            modelBuilder.Entity<LanguagePageReport>().HasNoKey().ToView(null);
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WikimediaProcessor.Data.Entities;

namespace WikimediaProcessor.Data.Repositories
{
    public class ActivityPageRepository : IActivityPageRepository
    {
        private const string wikimediaProcessorString = "WikimediaProcessor";
        private readonly IDbContextFactory<WikimediaContext> contextFactory;

        public ActivityPageRepository(IDbContextFactory<WikimediaContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task SaveActivityPageAsync(DateTime activityDate, string[] parts, string domain, string language)
        {
            var domainCode = parts.ElementAtOrDefault(0);
            var pageName = parts.ElementAtOrDefault(1);
            var viewCount = parts.ElementAtOrDefault(2);

            using var context = contextFactory.CreateDbContext();
            var page = await context.Pages.FirstOrDefaultAsync(p => p.DomainCode == domainCode && p.Name == pageName);
            if (page == null)
            {
                page = new Page
                {
                    Id = Guid.NewGuid(),
                    Active = true,
                    Domain = domain,
                    DomainCode = domainCode,
                    Language = language,
                    Name = pageName,
                    CreatedBy = wikimediaProcessorString,
                };
                await context.Pages.AddAsync(page);
            }

            var isCount = int.TryParse(viewCount, out var count);
            if (isCount)
            {
                var activity = await context.Activities
                    .FirstOrDefaultAsync(a => a.PageId == page.Id && a.ActivityDate == activityDate && a.Count == count);

                if (activity == null)
                {
                    activity = new Activity
                    {
                        Id = Guid.NewGuid(),
                        ActivityDate = activityDate,
                        Active = true,
                        PageId = page.Id,
                        Count = count,
                        CreatedBy = wikimediaProcessorString,
                    };
                    await context.Activities.AddAsync(activity);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}

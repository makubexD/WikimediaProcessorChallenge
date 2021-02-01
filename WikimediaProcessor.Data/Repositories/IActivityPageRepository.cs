using System;
using System.Threading.Tasks;

namespace WikimediaProcessor.Data.Repositories
{
    public interface IActivityPageRepository
    {
        Task SaveActivityPageAsync(DateTime activityDate, string[] parts, string domain, string language);
    }
}
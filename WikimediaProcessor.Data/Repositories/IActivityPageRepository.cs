using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WikimediaProcessor.Data.Entities;

namespace WikimediaProcessor.Data.Repositories
{
    public interface IActivityPageRepository
    {
        Task SaveActivityPageAsync(DateTime activityDate, string[] parts, string domain, string language);
        PageAll GetAllPages(DateTime activityDate, string[] parts, string domain, string language);
    }
}
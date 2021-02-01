using System;
using System.IO;
using System.Threading.Tasks;

namespace WikimediaProcessor.Services.Downloaders
{
    public interface IPageViewsDownloader
    {
        Task<FileInfo> DownloadPageViews(DateTime date, int hour);
    }
}
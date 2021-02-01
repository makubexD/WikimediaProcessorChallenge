using System;
using System.IO;
using System.Threading.Tasks;

namespace WikimediaProcessor.Services.Readers
{
    public interface IPageViewsReader
    {
        Task ReadContentsAndSaveRecords(FileInfo compressedFile, DateTime activityDate);
    }
}
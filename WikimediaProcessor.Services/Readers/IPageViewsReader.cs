using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WikimediaProcessor.Data.Entities;
using WikimediaProcessor.Services.Dto;

namespace WikimediaProcessor.Services.Readers
{
    public interface IPageViewsReader
    {
        Task ReadContentsAndSaveRecords(FileInfo compressedFile, DateTime activityDate);
        void ReadContentsAndSaveRecordsByBulk(FileInfo compressedFile, DateTime activityDate);
        List<PageAll> ReadContents(FileInfo compressedFile, DateTime activityDate);
        (List<LanguageDomainCount> languageDomainCounts, List<LanguagePageCount> languagePageCounts) Save(List<PageAll> pageAlls);
        void SaveDb(List<PageAll> pageAlls);
        Task SaveLanguageDomainDataPerDay(DateTime activityDate, DateTime endDate, List<LanguageDomainCount> languageDomainCounts, string prefix = "LanguageDomain");
        Task SaveLanguagePageDataPerDay(DateTime activityDate, DateTime endDate, List<LanguagePageCount> languagePageCounts, string prefix = "LanguagePage");
    }
}
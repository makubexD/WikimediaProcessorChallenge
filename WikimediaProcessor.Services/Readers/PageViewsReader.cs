using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharpCompress.Archives;
using SharpCompress.Common;
using WikimediaProcessor.Data.Entities;
using WikimediaProcessor.Data.Repositories;
using WikimediaProcessor.Services.Dto;

namespace WikimediaProcessor.Services.Readers
{
    public class PageViewsReader : IPageViewsReader
    {
        private readonly IPageAllRepository pageAllRepository;
        private readonly IActivityPageRepository activityPageRepository;
        private readonly IDomainCodeReader domainCodeReader;
        private readonly ILogger<PageViewsReader> logger;

        public PageViewsReader(IActivityPageRepository activityPageRepository, IDomainCodeReader domainCodeReader, ILogger<PageViewsReader> logger
            , IPageAllRepository pageAllRepository
            )
        {
            this.pageAllRepository = pageAllRepository;
            this.activityPageRepository = activityPageRepository;
            this.domainCodeReader = domainCodeReader;
            this.logger = logger;
        }

        public async Task ReadContentsAndSaveRecords(FileInfo compressedFile, DateTime activityDate)
        {
            int lineCount = 0;
            var textFile = GetTextFile(compressedFile);

            try
            {
                //using (var stream = new FileStream(textFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var stream = new FileStream(textFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true))
                {
                    using TextReader reader = new StreamReader(stream);
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        try
                        {
                            var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                            var domainCode = parts.ElementAtOrDefault(0);
                            var (domain, language) = domainCodeReader.GetDomainAndLanguage(domainCode);
                            await activityPageRepository.SaveActivityPageAsync(activityDate, parts, domain, language);
                            lineCount++;
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"Error while reading line {lineCount + 1}: {ex.Message}");
                            throw;
                        }
                    }
                }
                logger.LogInformation($"{lineCount} lines were read and saved from text file {textFile.Name}");
            }
            finally
            {
                if (textFile != null)
                    textFile.Delete();
            }
        }

        private static FileInfo GetTextFile(FileInfo compressedFile)
        {
            using var stream = new FileStream(compressedFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var archive = ArchiveFactory.Open(stream);
            var entry = archive.Entries.Single();
            var destinationFile = new FileInfo(Path.GetFileName(entry.Key ?? Guid.NewGuid().ToString()));
            entry.WriteToFile(destinationFile.FullName, new ExtractionOptions { ExtractFullPath = false, Overwrite = true });
            return destinationFile;
        }

        public void ReadContentsAndSaveRecordsByBulk(FileInfo compressedFile, DateTime activityDate)
        {
            int lineCount = 0;
            var textFile = GetTextFile(compressedFile);


            var paginas = new List<PageAll>();

            try
            {
                //using (var stream = new FileStream(textFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var stream = new FileStream(textFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true))                
                {
                    using TextReader reader = new StreamReader(stream);
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        try
                        {
                            lineCount++;
                            var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                            if (parts.Length != 4) {
                                
                                logger.LogWarning($"Error while reading line {lineCount}:  -->  {string.Join(" - ", string.IsNullOrEmpty(line) ? "Problem: doesn't accomplish the correct structure" : line)}");
                                continue;
                            }                           

                            var domainCode = parts.ElementAtOrDefault(0);
                            var (domain, language) = domainCodeReader.GetDomainAndLanguage(domainCode);


                            paginas.Add(activityPageRepository.GetAllPages(activityDate, parts, domain, language));
                        }
                        catch (Exception ex)
                        {                            
                            logger.LogError(ex, $"Error while reading line {lineCount}: {ex.Message}");
                            throw;
                        }
                    }
                }

                SaveDb(paginas);

                logger.LogInformation($"{lineCount} lines were read and saved from text file {textFile.Name}");
            }
            finally
            {
                if (textFile != null)
                    textFile.Delete();
            }

        }

        public List<PageAll> ReadContents(FileInfo compressedFile, DateTime activityDate)
        {
            int lineCount = 0;
            var textFile = GetTextFile(compressedFile);


            var paginas = new List<PageAll>();

            try
            {
                //using (var stream = new FileStream(textFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var stream = new FileStream(textFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true))                
                {
                    using TextReader reader = new StreamReader(stream);
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        try
                        {
                            lineCount++;
                            var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                            if (parts.Length != 4)
                            {

                                logger.LogWarning($"Error while reading line {lineCount}:  -->  {string.Join(" - ", string.IsNullOrEmpty(line) ? "Problem: doesn't accomplish the correct structure" : line)}");
                                continue;
                            }

                            var domainCode = parts.ElementAtOrDefault(0);
                            var (domain, language) = domainCodeReader.GetDomainAndLanguage(domainCode);


                            paginas.Add(activityPageRepository.GetAllPages(activityDate, parts, domain, language));
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"Error while reading line {lineCount}: {ex.Message}");
                            throw;
                        }
                    }
                }

                logger.LogInformation($"{lineCount} lines were read and saved from text file {textFile.Name}");
                return paginas;
            }
            finally
            {
                if (textFile != null)
                    textFile.Delete();
            }
        }

        public (List<LanguageDomainCount> languageDomainCounts, List<LanguagePageCount> languagePageCounts) Save(List<PageAll> pageAlls)
        {
            var reportLanguageDomainCount = pageAlls
                    .GroupBy(dt => new { dt.PageDate, dt.Language, dt.Domain })
                    .Select(d => new LanguageDomainCount()
                    {
                        Period = d.Key.PageDate.ToString("yyyyMM"),
                        Language = d.Key.Language,
                        Domain = d.Key.Domain,
                        ViewCount = d.Sum(X => X.ViewCount)
                    }).ToList();

            var reportLanguagePageCount = pageAlls
                    .GroupBy(dt => new { dt.PageDate, dt.Language, dt.Name })
                    .Select(d => new LanguagePageCount()
                    {
                        Period = d.Key.PageDate.ToString("yyyyMM"),
                        Language = d.Key.Language,
                        Page = d.Key.Name,                        
                        ViewCount = d.Sum(X => X.ViewCount)
                    }).ToList();

            return (reportLanguageDomainCount, reportLanguagePageCount);
        }

        public void SaveDb(List<PageAll> pageAlls)
        {
            try
            {
                pageAllRepository.Create(pageAlls);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.Message);

                pageAllRepository.CreateByBatches(pageAlls);
            }
        }

        //TODO: REFACTOR TO PASS ONE GENERIC AND READ PROPERTIES MORE DYNAMIC
        public async Task SaveLanguageDomainDataPerDay(DateTime activityDate, DateTime endDate, List<LanguageDomainCount> languageDomainCounts, string prefix = "LanguageDomain")
        {
            BuildDirectory(activityDate, endDate, prefix);

            var folderName = $"{activityDate.Year}{activityDate.Month.ToString().PadLeft(2, '0')}_{prefix}";

            var fileNamePerDay = $"{folderName}{activityDate.Day.ToString().PadLeft(2, '0') }";

            DirectoryInfo dir = new DirectoryInfo(folderName);
            var file = Path.Combine(dir.FullName, fileNamePerDay);

            var fileInfo = new FileInfo(file);

            using (var stream = new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, true)) 
            {
                var bytes = languageDomainCounts.SelectMany(s => Encoding.UTF8.GetBytes(s.Period + "\t" + s.Language + "\t" + s.Domain + "\t" + s.ViewCount + Environment.NewLine)).ToArray();                
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }            
        }

        public async Task SaveLanguagePageDataPerDay(DateTime activityDate, DateTime endDate, List<LanguagePageCount> languagePageCounts, string prefix = "LanguagePage")
        {
            BuildDirectory(activityDate, endDate, prefix);

            var folderName = $"{activityDate.Year}{activityDate.Month.ToString().PadLeft(2, '0')}_{prefix}";

            var fileNamePerDay = $"{folderName}{activityDate.Day.ToString().PadLeft(2, '0') }";

            DirectoryInfo dir = new DirectoryInfo(folderName);
            var file = Path.Combine(dir.FullName, fileNamePerDay);

            var fileInfo = new FileInfo(file);

            using (var stream = new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, true))
            {
                var bytes = languagePageCounts.SelectMany(s => Encoding.UTF8.GetBytes(s.Period + "\t" + s.Language + "\t" + s.Page + "\t" + s.ViewCount + Environment.NewLine)).ToArray();
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }            
        }

        private void BuildDirectory(DateTime activityDate, DateTime endDate, string prefix)
        {
            var folderName = $"{activityDate.Year}{activityDate.Month.ToString().PadLeft(2, '0')}_{prefix}";

            //if (Directory.Exists(folderName))
            //    Directory.Delete(folderName, true);

            //Directory.CreateDirectory(folderName);

            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharpCompress.Archives;
using SharpCompress.Common;
using WikimediaProcessor.Data.Repositories;

namespace WikimediaProcessor.Services.Readers
{
    public class PageViewsReader : IPageViewsReader
    {
        private readonly IActivityPageRepository activityPageRepository;
        private readonly IDomainCodeReader domainCodeReader;
        private readonly ILogger<PageViewsReader> logger;

        public PageViewsReader(IActivityPageRepository activityPageRepository, IDomainCodeReader domainCodeReader, ILogger<PageViewsReader> logger)
        {
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
    }
}

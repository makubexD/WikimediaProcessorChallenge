using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;

namespace WikimediaProcessor.Services.Reporters
{
    public static class CsvHelper<T> where T : class
    {
        public static bool WriteToCsvFile(string csvFilePath, IEnumerable<T> records, ILogger logger)
        {
            if (records == null)
                throw new ArgumentNullException(nameof(records));

            try
            {
                using (var streamWriter = new StreamWriter(csvFilePath, false, Encoding.UTF8))
                {
                    var configuration = new CsvConfiguration(CultureInfo.CurrentCulture, shouldQuote: (field, context) => true);
                    using var csvWriter = new CsvWriter(streamWriter, configuration);
                    csvWriter.WriteRecords(records);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while writing the CSV file in {csvFilePath}");
                throw;
            }
        }
    }
}

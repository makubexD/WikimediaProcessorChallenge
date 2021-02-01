using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WikimediaProcessor.Data.MigrationHelpers
{
    [ExcludeFromCodeCoverage]
    internal static class MigrationHelper
    {
        internal static string GetSqlFromEmbeddedStoredProcedure(string embeddedSqlFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .Where(str => str.EndsWith(embeddedSqlFileName))
                .FirstOrDefault();
            if (!string.IsNullOrEmpty(resourceName))
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream);
                var sql = reader.ReadToEnd();
                sql = sql.Replace("'", "''");
                return sql;
            }

            throw new InvalidOperationException($"There was no embedded SQL file with name: {embeddedSqlFileName} in assembly {assembly}");
        }
    }
}

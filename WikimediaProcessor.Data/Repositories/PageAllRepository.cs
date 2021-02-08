using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using WikimediaProcessor.Data.Entities;
using System;
using System.Linq;

namespace WikimediaProcessor.Data.Repositories
{
    public class PageAllRepository : Repository, IPageAllRepository
    {
        public PageAllRepository(string conex)
        {
            _context = new SqlConnection(conex);
        }

        public void Create(List<PageAll> pageAll)
        {
            try
            {
                var dt = GetDataTableFrom(pageAll);
                BulkLoadData(dt, _context);
            }
            catch (Exception)
            {                
                throw new Exception("Failed but we are going to try once...");
            }
        }

        public void CreateByBatches(List<PageAll> pageAll)
        {
            try
            {                
                InsertByMultipleDatatableFrom(pageAll);
            }
            catch (Exception)
            {

                throw new Exception("Failed, even with a second try");
            }            
        }

        public DataTable GetDataTableFrom(List<PageAll> pageAll)
        {
            var dt = BuildPageAllDataTableTemplate();

            foreach (var page in pageAll)
            {
                dt.Rows.Add(page.Id, page.Name, page.Domain, page.Language, page.DomainCode, page.PageDate, page.ViewCount);
            }
            
            return dt;
        }

        public void InsertByMultipleDatatableFrom(List<PageAll> pageAll)
        {            
            const int splitter = 500000;
            for (int i = 0; i < pageAll.Count; i += splitter)
            {
                var items = pageAll.Skip(i).Take(splitter);

                var dt = BuildPageAllDataTableTemplate();

                foreach (var page in items)
                {
                    dt.Rows.Add(page.Name, page.Domain, page.Language, page.DomainCode, page.PageDate, page.ViewCount);
                }

                BulkLoadData(dt, _context);
            }
        }

        private DataTable BuildPageAllDataTableTemplate() 
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(long));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Domain", typeof(string));
            dt.Columns.Add("Language", typeof(string));
            dt.Columns.Add("DomainCode", typeof(string));
            dt.Columns.Add("PageDate", typeof(DateTime));
            dt.Columns.Add("ViewCount", typeof(int));

            return dt;
        }

        private void BulkLoadData(DataTable dt, SqlConnection context)
        {            
            using (SqlConnection conn = new SqlConnection(context.ConnectionString))
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.TableLock, null))            
            {
                bulkCopy.DestinationTableName = "[dbo].[PageAll]";
                bulkCopy.BulkCopyTimeout = 1200;
                bulkCopy.ColumnMappings.Add("Id", "Id");
                bulkCopy.ColumnMappings.Add("Name", "Name");
                bulkCopy.ColumnMappings.Add("Domain", "Domain");
                bulkCopy.ColumnMappings.Add("Language", "Language");
                bulkCopy.ColumnMappings.Add("DomainCode", "DomainCode");
                bulkCopy.ColumnMappings.Add("PageDate", "PageDate");
                bulkCopy.ColumnMappings.Add("ViewCount", "ViewCount");
                conn.Open();
                bulkCopy.WriteToServer(dt);                
            }
        }        
    }
}

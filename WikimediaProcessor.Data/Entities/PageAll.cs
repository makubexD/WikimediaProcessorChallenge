using System;

namespace WikimediaProcessor.Data.Entities
{
    public class PageAll
    {
        public long Id { get; set; }
        public string Name { get; set; }        
        public string Domain { get; set; }        
        public string Language { get; set; }        
        public string DomainCode { get; set; }
        public DateTime PageDate { get; set; }
        public int ViewCount { get; set; }
    }
}

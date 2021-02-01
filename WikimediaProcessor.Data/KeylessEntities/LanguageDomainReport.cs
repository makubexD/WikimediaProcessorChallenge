using Microsoft.EntityFrameworkCore;

namespace WikimediaProcessor.Data.KeylessEntities
{
    [Keyless]
    public class LanguageDomainReport
    {
        public string Period { get; set; }
        public string Language { get; set; }
        public string Domain { get; set; }
        public int Count { get; set; }
    }
}

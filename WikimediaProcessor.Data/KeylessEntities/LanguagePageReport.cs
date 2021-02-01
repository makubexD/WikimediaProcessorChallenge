using Microsoft.EntityFrameworkCore;

namespace WikimediaProcessor.Data.KeylessEntities
{
    [Keyless]
    public class LanguagePageReport
    {
        public string Period { get; set; }
        public string Language { get; set; }
        public string Page { get; set; }
        public int Count { get; set; }
    }
}

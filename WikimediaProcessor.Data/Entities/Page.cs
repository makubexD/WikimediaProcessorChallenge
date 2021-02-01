using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WikimediaProcessor.Data.Entities
{
    [Index(nameof(Name), nameof(DomainCode))]
    public class Page : BaseEntity
    {
        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Domain { get; set; }

        [StringLength(250)]
        public string Language { get; set; }

        [StringLength(250)]
        public string DomainCode { get; set; }

        public ICollection<Activity> Activities { get; set; }
    }
}

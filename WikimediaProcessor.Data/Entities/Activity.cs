using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WikimediaProcessor.Data.Entities
{
    public class Activity : BaseEntity
    {
        [Column(TypeName = "date")]
        public DateTime ActivityDate { get; set; }
        public int? Count { get; set; }
        public Guid PageId { get; set; }
        public Page Page { get; set; }
    }
}

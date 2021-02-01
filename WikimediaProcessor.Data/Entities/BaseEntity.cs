using System;
using System.ComponentModel.DataAnnotations;

namespace WikimediaProcessor.Data.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public bool? Active { get; set; }

        public DateTime Created { get; set; }

        [StringLength(50)]
        [Required]
        public string CreatedBy { get; set; }

        public DateTime? Modified { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}

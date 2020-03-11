using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MediaBalans.Models
{
    public class MutualForPostComment
    {
        [Key]
        public int Id { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAtTime { get; set; }
        public DateTime UpdatedAtTime { get; set; }
        public DateTime DeletedAtTime { get; set; }
        public string CustomUserId { get; set; }
        [ForeignKey("CustomUserId")]
        public virtual CustomUser CustomUser { get; set; }
    }
}

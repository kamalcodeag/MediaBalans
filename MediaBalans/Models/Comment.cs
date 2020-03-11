using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MediaBalans.Models
{
    public class Comment : MutualForPostComment
    {
        [Required(ErrorMessage = "Zəhmət olmasa, boşluğu doldurun"), StringLength(2500, ErrorMessage = "Yazının uzunluğu 2500 simvoldan uzun ola bilməz")]
        public string ComContent { get; set; }
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }
    }
}

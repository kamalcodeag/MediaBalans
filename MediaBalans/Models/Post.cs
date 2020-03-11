using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MediaBalans.Models
{
    public class Post : MutualForPostComment
    {
        [Required(ErrorMessage = "Zəhmət olmasa, boşluğu doldurun"), StringLength(2500, ErrorMessage = "Yazının uzunluğu 2500 simvoldan uzun ola bilməz")]
        public string PostContent { get; set; }
        public int LikesCount { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}

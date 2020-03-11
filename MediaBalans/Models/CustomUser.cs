using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaBalans.Models
{
    public class CustomUser : IdentityUser
    {
        [Required(ErrorMessage = "Zəhmət olmasa, boşluğu doldurun"), StringLength(50, ErrorMessage = "Yazının uzunluğu 50 simvoldan uzun ola bilməz")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Zəhmət olmasa, boşluğu doldurun"), StringLength(50, ErrorMessage = "Yazının uzunluğu 50 simvoldan uzun ola bilməz")]
        public string LastName { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }
        public virtual ICollection<ConnectedUser> ConnectedUsers { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}

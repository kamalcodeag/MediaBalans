using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MediaBalans.Models
{
    public class ConnectedUser
    {
        [Key]
        public int Id { get; set; }
        public string FollowingId { get; set; }
        [ForeignKey("FollowingId")]
        public virtual CustomUser Following { get; set; }
        public string FollowedById { get; set; }
    }
}

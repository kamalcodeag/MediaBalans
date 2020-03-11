using MediaBalans.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaBalans.ViewModels
{
    public class MyPostsVM
    {
        public Post Post { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public IQueryable<Comment> Comments { get; set; }
    }
}

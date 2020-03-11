using MediaBalans.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaBalans.DAL
{
    public class MediaBalansDb : IdentityDbContext<CustomUser>
    {
        public MediaBalansDb(DbContextOptions<MediaBalansDb> options) : base(options)
        {
        }

        public DbSet<ConnectedUser> ConnectedUsers { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        //Using Lazy Loading
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies()
                          .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.DetachedLazyLoadingWarning));
        }
    }
}

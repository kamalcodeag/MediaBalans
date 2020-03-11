using MediaBalans.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaBalans.ViewModels
{
    public class FollowVM
    {
        public IQueryable<ConnectedUser> MyFollowingUsers { get; set; }
        public List<CustomUser> FilteredCustomUsers { get; set; }
        public List<string> PairedUsers { get; set; }
    }
}

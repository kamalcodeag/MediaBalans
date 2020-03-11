using MediaBalans.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaBalans.Utilities
{
    public static class MainUtility
    {
        public static CustomUser currentUser { get; set; }
        public static async Task<CustomUser> FindAnActiveUser(UserManager<CustomUser> _userManager, string userName)
        {
            try
            {
                currentUser = await _userManager.FindByNameAsync(userName);
            }
            catch(Exception ex)
            {
                ex.ToString();
            }
            return currentUser;
        }
    }
}

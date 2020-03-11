using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaBalans.DAL;
using MediaBalans.Models;
using MediaBalans.Utilities;
using MediaBalans.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MediaBalans.Controllers
{
    public class ProfileController : Controller
    {
        private readonly MediaBalansDb _context;
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        public CustomUser currentUser { get; set; }

        public ProfileController(MediaBalansDb context,
                                  UserManager<CustomUser> userManager,
                                  SignInManager<CustomUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> ChangePassword()
        {
            currentUser = await MainUtility.FindAnActiveUser(_userManager, User.Identity.Name);
            ViewBag.CurrentUser = currentUser.Id;

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM changePasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordVM);
            }

            currentUser = await MainUtility.FindAnActiveUser(_userManager, User.Identity.Name);
            CustomUser customUserFromDb = await _userManager.FindByIdAsync(currentUser.Id);

            IdentityResult result = await _userManager.ChangePasswordAsync(customUserFromDb, changePasswordVM.CurrentPassword, changePasswordVM.NewPassword);
            await _userManager.UpdateAsync(customUserFromDb);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Hazırki şifrə yanlışdır və ya yeni şifrə tələblərə uyğun deyil.");
                return View(changePasswordVM);
            }

            TempData["PasswordChanged"] = true;
            return RedirectToAction("MyPosts", "Profile", new { userId = currentUser.Id });
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Home");
        }

        public async Task<IActionResult> MyPosts(string userId)
        {
            ViewBag.ActivePage = "MyPosts";

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            currentUser = await MainUtility.FindAnActiveUser(_userManager, User.Identity.Name);
            CustomUser customUserFromDb = await _userManager.FindByIdAsync(userId);

            if(currentUser == customUserFromDb)
            {
                ViewBag.AuthorOfPosts = 1;
            }

            ViewBag.CurrentUser = currentUser.Id;

            MyPostsVM myPostsVM = new MyPostsVM
            {
                Posts = customUserFromDb.Posts.Where(po => po.IsDeleted == false).OrderByDescending(p => p.UpdatedAtTime > p.CreatedAtTime ? p.UpdatedAtTime : p.CreatedAtTime)
            };

            return View(myPostsVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(MyPostsVM myPostsVM)
        {
            currentUser = await MainUtility.FindAnActiveUser(_userManager, User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return RedirectToAction("MyPosts", "Profile", new { userId = currentUser.Id });
            }

            Post post = new Post
            {
                PostContent = myPostsVM.Post.PostContent.Trim(),
                CreatedAtTime = DateTime.Now,
                CustomUserId = currentUser.Id
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyPosts", "Profile", new { userId = currentUser.Id });
        }

        [Authorize]
        public async Task<IActionResult> UpdatePost(int? id)
        {
            currentUser = await MainUtility.FindAnActiveUser(_userManager, User.Identity.Name);
            ViewBag.CurrentUser = currentUser.Id;

            if (id == null)
            {
                return NotFound();
            }

            Post post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            if (currentUser != post.CustomUser)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePost(int? id, Post inputPost)
        {
            currentUser = await MainUtility.FindAnActiveUser(_userManager, User.Identity.Name);

            if (id == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("MyPosts", "Profile", new { userId = currentUser.Id });
            }

            Post post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            if (currentUser != post.CustomUser)
            {
                return NotFound();
            }

            post.PostContent = inputPost.PostContent.Trim();
            post.IsUpdated = true;
            post.UpdatedAtTime = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction("MyPosts", "Profile", new { userId = currentUser.Id });
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> DeletePost(int? id)
        {
            currentUser = await MainUtility.FindAnActiveUser(_userManager, User.Identity.Name);

            if (id == null)
            {
                return NotFound();
            }

            Post post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            if (currentUser != post.CustomUser)
            {
                return NotFound();
            }

            post.IsDeleted = true;
            post.DeletedAtTime = DateTime.Now;
            await _context.SaveChangesAsync();

            return Json(new { success = 200 });
        }

        [Authorize]
        public async Task<IActionResult> Following()
        {
            ViewBag.ActivePage = "Following";

            currentUser = await MainUtility.FindAnActiveUser(_userManager, User.Identity.Name);
            ViewBag.CurrentUser = currentUser.Id;

            IQueryable<ConnectedUser> myFollowingUsers = _context.ConnectedUsers.Where(cu => cu.FollowedById == currentUser.Id);

            FollowVM followVM = new FollowVM
            {
                MyFollowingUsers = myFollowingUsers
            };

            return View(followVM);
        }

        [Authorize]
        public async Task<IActionResult> Followers()
        {
            ViewBag.ActivePage = "Followers";

            currentUser = await MainUtility.FindAnActiveUser(_userManager, User.Identity.Name);
            ViewBag.CurrentUser = currentUser.Id;

            IQueryable<ConnectedUser> myFollowers = _context.ConnectedUsers.Where(cu => cu.FollowingId == currentUser.Id);
            List<CustomUser> filteredCustomUsers = new List<CustomUser>();

            foreach (CustomUser customUser in _userManager.Users)
            {
                if (myFollowers.Count() > 0 && customUser.Id != currentUser.Id)
                {
                    foreach (ConnectedUser myFollower in myFollowers)
                    {
                        if (customUser.Id == myFollower.FollowedById)
                        {
                            filteredCustomUsers.Add(customUser);
                        }
                    }
                }
            }

            FollowVM followVM = new FollowVM
            {
                FilteredCustomUsers = filteredCustomUsers,
                PairedUsers = new List<string>()
            };

            foreach (ConnectedUser connectedUser in _context.ConnectedUsers)
            {
                foreach (ConnectedUser myFollower in myFollowers)
                {
                    if (connectedUser.FollowingId == myFollower.FollowedById && connectedUser.FollowedById == currentUser.Id)
                    {
                        followVM.PairedUsers.Add(myFollower.FollowedById);
                        break;
                    }
                    //else
                    //{
                    //    break;
                    //}
                }
            }

            return View(followVM);
        }

        [Authorize]
        public async Task<IActionResult> ExploreNewUsers()
        {
            ViewBag.ActivePage = "ExploreNewUsers";

            currentUser = await MainUtility.FindAnActiveUser(_userManager, User.Identity.Name);
            ViewBag.CurrentUser = currentUser.Id;

            IQueryable<ConnectedUser> myFollowingUsers = _context.ConnectedUsers.Where(cu => cu.FollowedById == currentUser.Id);
            List<CustomUser> filteredCustomUsers = new List<CustomUser>();
            CustomUser duplicateUser = null;
            foreach (CustomUser customUser in _userManager.Users)
            {
                if (myFollowingUsers.Count() > 0 && customUser.Id != currentUser.Id)
                {
                    foreach (ConnectedUser myFollowingUser in myFollowingUsers)
                    {
                        if (customUser.Id != myFollowingUser.FollowingId)
                        {
                            filteredCustomUsers.Add(customUser);
                            duplicateUser = customUser;
                            //break;
                        }
                        else
                        {
                            if(duplicateUser == customUser)
                            {
                                filteredCustomUsers.Remove(duplicateUser);
                            }
                            break;
                        }
                    }
                }
                else if (myFollowingUsers.Count() == 0 && customUser.Id != currentUser.Id)
                {
                    filteredCustomUsers.Add(customUser);
                }
            }

            for (int i = 0; i < filteredCustomUsers.Count(); i++)
            {
                if(i != filteredCustomUsers.Count())
                {
                    if(filteredCustomUsers.ElementAt(i) == filteredCustomUsers.ElementAt(i+1))
                    {
                        filteredCustomUsers.Remove(filteredCustomUsers.ElementAt(i));
                    }
                }
            }

            FollowVM followVM = new FollowVM
            {
                FilteredCustomUsers = filteredCustomUsers
            };

            return View(followVM);
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> FollowUser(string userId)
        {
            currentUser = await MainUtility.FindAnActiveUser(_userManager, User.Identity.Name);

            if (userId == null)
            {
                return NotFound();
            }

            ConnectedUser connectedUser = new ConnectedUser
            {
                FollowingId = userId,
                FollowedById = currentUser.Id
            };

            _context.ConnectedUsers.Add(connectedUser);
            await _context.SaveChangesAsync();
            await _context.SaveChangesAsync();

            return Json(new { success = 200 });
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> UnFollowUser(string userId)
        {
            currentUser = await MainUtility.FindAnActiveUser(_userManager, User.Identity.Name);

            if (userId == null)
            {
                return NotFound();
            }

            ConnectedUser connectedUserToBeUnFollowed = _context.ConnectedUsers.FirstOrDefault(cu => cu.FollowingId == userId && cu.FollowedById == currentUser.Id);
            _context.Remove(connectedUserToBeUnFollowed);
            await _context.SaveChangesAsync();

            return Json(new { success = 200 });
        }
    }
}
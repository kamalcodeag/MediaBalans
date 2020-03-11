using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediaBalans.Models;
using MediaBalans.DAL;
using Microsoft.AspNetCore.Identity;
using MediaBalans.ViewModels;
using MediaBalans.Utilities;
using Microsoft.Extensions.Localization;
using MediaBalans.Resources;

namespace MediaBalans.Controllers
{
    public class HomeController : Controller
    {
        private readonly MediaBalansDb _context;
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        public CustomUser currentUser { get; set; }

        public HomeController(MediaBalansDb context,
                              UserManager<CustomUser> userManager,
                              SignInManager<CustomUser> signInManager,
                              IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _sharedLocalizer = sharedLocalizer;
        }
        public async Task<IActionResult> Login()
        {
            currentUser = await MainUtility.FindAnActiveUser(_userManager, User.Identity.Name);

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MyPosts", "Profile", new { userId = currentUser.Id });
            }

            var loginTitle = _sharedLocalizer["msg"];
            //ViewBag.LoginTitle = loginTitle;

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            CustomUser customUserFromDb = await _userManager.FindByNameAsync(loginVM.UserName);

            if (customUserFromDb == null)
            {
                ModelState.AddModelError("", "Daxil etdiyiniz istifadəçi adı və ya şifrə yanlışdır.");
                return View(loginVM);
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(customUserFromDb, loginVM.Password, loginVM.RememberMe, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Daxil etdiyiniz istifadəçi adı və ya şifrə yanlışdır.");
                return View(loginVM);
            }

            return RedirectToAction("MyPosts", "Profile", new { userId = customUserFromDb.Id });
        }
        public async Task<IActionResult> Register()
        {
            currentUser = await MainUtility.FindAnActiveUser(_userManager, User.Identity.Name);

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MyPosts", "Profile", new { userId = currentUser.Id });
            }

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            CustomUser customUser = new CustomUser
            {
                FirstName = registerVM.FirstName.Trim(),
                LastName = registerVM.LastName.Trim(),
                UserName = registerVM.UserName.Trim()
            };

            IdentityResult result = await _userManager.CreateAsync(customUser, registerVM.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Bu istifadəçi artıq qeydiyyatdan keçib və ya şifrə tələblərə uyğun deyil.");
                return View(registerVM);
            }

            await _context.SaveChangesAsync();

            TempData["Registered"] = true;
            return RedirectToAction("Login", "Home");
        }
    }
}

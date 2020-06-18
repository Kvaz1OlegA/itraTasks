using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using task4.Models;
using task4.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System;

namespace task4.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public User CurrentUser { get; set; }

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        public IActionResult Index() => View(_userManager.Users.ToList());

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            CurrentUser = new User { Email = model.Email, UserName = model.Email, Name = model.Name, registrationDate = DateTime.Now, lastLoginDate = DateTime.Now, Blocked = false };
           
            if (ModelState.IsValid)
            { 
                var result = await _userManager.CreateAsync(CurrentUser, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(CurrentUser, false);
                    return RedirectToAction("Index", "Users");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            CurrentUser = await _signInManager.UserManager.FindByEmailAsync(model.Email);
            if (ModelState.IsValid)
            {
                var result =
                await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!CurrentUser.Blocked)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            CurrentUser.lastLoginDate = DateTime.Now;
                            var LogUpdate = await _userManager.UpdateAsync(CurrentUser);
                            return RedirectToAction("Index", "Users");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            CurrentUser = null;
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            if (id == CurrentUser.Id)
            {
                CurrentUser = null;
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Block(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Blocked = true;
                var LogUpdate = await _userManager.UpdateAsync(user);
            }
            if (id == CurrentUser.Id)
            {
                CurrentUser = null;
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Unblock(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Blocked = false;
                var LogUpdate = await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Index");
        }
    }
}
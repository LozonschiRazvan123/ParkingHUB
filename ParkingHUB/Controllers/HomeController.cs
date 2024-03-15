using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingHUB.Data;
using ParkingHUB.Models;
using ParkingHUB.ViewModel;
using System.Diagnostics;

namespace ParkingHUB.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> ProcessLogin(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);

            var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);

            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Parking");
                    }
                }
                TempData["Error"] = "Wrong credentials. Please try again";
                return View("Login", loginViewModel); 
            }
            TempData["Error"] = "Wrong credentials. Please try again";
            return View("Login", loginViewModel); 
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();   
            return RedirectToAction("Login", "Home");

        }

        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        public async Task<IActionResult> ProcessRegister(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(registerViewModel);

            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(registerViewModel);
            }

            var newUser = new User()
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.UserName
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (newUserResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                await _signInManager.SignInAsync(newUser, isPersistent: false); 
                return RedirectToAction("Index", "Parking");
            }

            return View(registerViewModel);
        }

    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingHUB.Data;
using ParkingHUB.DTO;
using ParkingHUB.Interface;
using ParkingHUB.Models;
using ParkingHUB.ViewModel;
using System.Diagnostics;

namespace ParkingHUB.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmail _emailSender;
        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager, IEmail emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
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
                        var userId = user.Id;
                        return RedirectToAction("Index", "Parking", new {userId = userId});
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

        [HttpPost]
        public async Task<IActionResult> ProcessRegister(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", registerViewModel); 
            }

            var userExists = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            if (userExists != null)
            {
                TempData["Error"] = "The user already exist!";
                return View("Register", registerViewModel);
            }
            var newUser = new User
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.UserName
            };

            var result = await _userManager.CreateAsync(newUser, registerViewModel.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);

                await _signInManager.SignInAsync(newUser, isPersistent: false);

                return RedirectToAction("Index", "Parking");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Register", registerViewModel);
            }
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.EmailAddress);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ResetPassword", "Home", new { userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme); 
                    var emailDto = new EmailDTO
                    {
                        From = "parkinhub@hub.ro",
                        To = model.EmailAddress,
                        Subject = "Reset Password",
                        Body = $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>."
                    };

                    _emailSender.SendEmail(emailDto);

                    return Ok();
                }
            }

            return BadRequest();
        }


        [HttpGet] 
        public IActionResult ResetPassword(string token)
        {
            var model = new ResetPassword { Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string userId, string token, ResetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Login", "Home");
            }
            if (model.Password != model.ConfirmPassword)
            {
                TempData["Error"] = "The password and confirmation password do not match.";
                return View("ResetPassword", model);
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return View("ResetPassword", model);;
            }

            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                TempData["Error"] = "An error occurred while resetting the password.";
                return View("ResetPassword", model); ;
            }
        }
    }
}

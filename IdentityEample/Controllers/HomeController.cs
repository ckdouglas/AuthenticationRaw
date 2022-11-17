using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;
        public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index() => View();

        [Authorize]
        public IActionResult Secret() => View();

        public IActionResult Login() => View();

        public IActionResult Register() => View();

        public IActionResult EmailVerification() => View();

        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new IdentityUser
            {
                UserName = username,
                Email = "",
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                //generation of email token
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var link = Url.Action(nameof(VerifyEmail), "Home", new { userId = user.Id, code }, Request.Scheme, Request.Host.Value);

                await _emailService.SendAsync("test@test.com", "Email verify", $"<a href=\"{link}\">Verify Email</a>", true);

                return RedirectToAction("EmailVerification");

            }
            return View();
        }


        public async Task<IActionResult> VerifyEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest();
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return View();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return BadRequest();
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var link = Url.Action(nameof(ResetPassword), "Home", new { userId = user.Id, code }, Request.Scheme, Request.Host.Value);

            await _emailService.SendAsync("test@test.com", "Password Reset", $"<a href=\"{link}\">Reset Password</a>", true);

            return View("PasswordResetEmailSent");
        }


        public async Task<IActionResult> ResetPassword(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest();
            if (!await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, UserManager<IdentityUser>.ResetPasswordTokenPurpose, code))
            {
                return BadRequest();
            }
            return View();
            // var result = _userManager.ResetPasswordAsync(user, code, password);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string userId, string code, string password)
        {
            //
            return null;
        }




        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}


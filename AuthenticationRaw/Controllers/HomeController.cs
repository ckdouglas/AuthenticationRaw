using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationRaw.Controllers
{
    public class HomeController : Controller
    {

        private readonly IAuthorizationService _authorizationService;

        public HomeController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }


        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "Claim.DoB")]
        public IActionResult Secret()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            var grandmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Cks"),
                new Claim(ClaimTypes.Email, "Cks"),
                new Claim(ClaimTypes.DateOfBirth, "11/11/2011"),
                new Claim("Ann.Says", "Hello Cks"),
            };


            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "CkDouglas"),
                new Claim("DrivingLicense","A+")
            };

            var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Ntsa");

            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenseIdentity });

            HttpContext.SignInAsync(userPrincipal);
            return View();
        }

        public async Task<IActionResult> DoStuff()
        {

            //we are doing stuff here
            var builder = new AuthorizationPolicyBuilder("Schema");
            var customPolicy = builder.RequireClaim("Hello").Build();
            var authorised = await _authorizationService.AuthorizeAsync(HttpContext.User, customPolicy);
            if (authorised.Succeeded)
            {
                //do something
            }
            return View("Index");
        }
    }
}

/**
      EXTRAS
1. IAuthorizationService - strategically place the authorization requests.

 */
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Extension;
using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static OnlineStore.Service.AccountService;

namespace OnlineStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICustomExtension _customExtension;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _accountService;

        public AccountController(ICustomExtension customExtension, IHttpContextAccessor httpContextAccessor,
            IAccountService accountService)
        {
            _customExtension = customExtension;
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
        }

        // GET: AccountController
        [AllowAnonymous]
        public IActionResult Login()
        {
            var user = new User();
            return PartialView(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Login(string loginUser, string loginPassword, bool loginRemember)
        {
            var status = new Status();
            status.Success = false;

            if (loginUser.Trim() != null && loginPassword.Trim() != null)
            {
                var userData = _accountService.IsUserValid(loginUser, loginPassword);

                if (userData != null)
                {

                    var roleList = _accountService.RoleList();

                    var role = roleList.Where(r => r.Id == userData.RoleId).Select(s => s.RoleName).FirstOrDefault();

                    var claims = new List<Claim>
                             {
                                 new Claim(ClaimTypes.Sid,  userData.Id.ToString()),
                                 new Claim(ClaimTypes.Role,  role),
                                 new Claim(ClaimTypes.Name,  userData.Name.ToString()),

                             };


                    _httpContextAccessor.HttpContext.Session.SetString("UserId", userData.Id.ToString()); //set session value

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    var authProperties = new AuthenticationProperties();
                    authProperties.ExpiresUtc = DateTime.UtcNow.AddMinutes(30);
                    if (loginRemember == true)
                        authProperties.IsPersistent = true;


                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                    status.Success = true;
                    status.Message = userData.Name.ToString();
                }
                else
                {
                    status.Success = false;
                    status.Message = "Invalid credentials";
                }

            }
            else
            {
                TempData["Error"] = "Invalid credentials";
            }

            return Json(status);
        }

        [Authorize]
        public ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.SignOutAsync();
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public JsonResult IsAutheticated()
        {
            var status = new Status();
            status.Success = false;
            if (User.Identity.IsAuthenticated)
            {
                status.Success = true;
            }
            return Json(status.Success);
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }


    }

}

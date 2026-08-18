using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels.LoginUser;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace CKM_ManagementSystem.Controllers.LoginUser
{
    public class LoginUsersController : Controller
    {
        private readonly LoginUserBL _loginUserBL;
        public LoginUsersController(LoginUserBL loginUserBL)
        {
            _loginUserBL = loginUserBL;
        }
        public static class CustomClaimTypes
        {
            public const string staffCode = "StaffCode";
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? staffCode)
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var result = _loginUserBL.LoginUser_select(model);
            if (result.IsSuccess)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, result.UserEmail),
                    new Claim("StaffCode", result.Staff_Code ?? ""),
                };
                var cliamsIdentity = new ClaimsIdentity
                (
                    claims, "MyCookieAuth"
                );
                await HttpContext.SignInAsync
                (
                    "MyCookieAuth", new ClaimsPrincipal(cliamsIdentity)
                );
                return Content($"Login Success! Logged in as: {result.UserEmail}, Staff Code: {result.Staff_Code}");
            }
            ViewBag.ErrorMessage = result.Message;
            return View(model);
        }

    }
}

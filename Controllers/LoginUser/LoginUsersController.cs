using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels.LoginUser;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
namespace CKM_ManagementSystem.Controllers.LoginUser
{
    public class LoginUsersController : Controller
    {
        private readonly LoginUserBL _loginUserBL;
        private readonly MainMenuBL _mainMenuBL;
        private readonly UserPermissionBL _userPermissionBL;
        public LoginUsersController(
            LoginUserBL loginUserBL,
            MainMenuBL mainMenuBL,
            UserPermissionBL userPermissionBL)
        {
            _loginUserBL = loginUserBL;
            _mainMenuBL = mainMenuBL;
            _userPermissionBL = userPermissionBL;
        }
        public static class CustomClaimTypes
        {
            public const string staffCode = "StaffCode";
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
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
                var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);
                var permissions = await _userPermissionBL
                    .GetPermissionsAsync(result.Staff_Code ?? "");
                HttpContext.Session.SetString(
                    "UserPermissions",
                    JsonSerializer.Serialize(permissions));
                var menus = await _mainMenuBL.GetMainMenus(
                    result.Staff_Code ?? "");
                var firstMenu = menus.SelectMany(
                    m => m.SubMenus != null && m.SubMenus.Count > 0
                    ? m.SubMenus
                    : new List<CKM_ManagementSystem.Models.ViewModels.MainMenu.MainMenuViewModel>
                    {
                        m
                    })
                    .FirstOrDefault(m =>
                     !string.IsNullOrWhiteSpace(m.ControllerName) &&
                     !string.IsNullOrWhiteSpace(m.ActionName));
                if (firstMenu != null)
                {
                    return RedirectToAction(
                        firstMenu.ActionName,
                        firstMenu.ControllerName);
                }
                ViewBag.ErrorMessage = "No menu permission assigned.";
                return View(model);
            }
            ViewBag.ErrorMessage = result.Message;
            model.Password = string.Empty;
            return View(model);
        }
    }
}
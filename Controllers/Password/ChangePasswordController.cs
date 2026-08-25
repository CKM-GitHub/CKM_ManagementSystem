using CKM_ManagementSystem.BL.Interface;
using CKM_ManagementSystem.Models.ViewModels.Password;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CKM_ManagementSystem.Controllers.Password
{
    [Authorize]
    [Route("Password")]
    public class ChangePasswordController : Controller
    {
        private readonly IChangePasswordBL _changePasswordBL;

        public ChangePasswordController(IChangePasswordBL changePasswordBL)
        {
            _changePasswordBL = changePasswordBL;
        }
        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            var staffCode = HttpContext.Session.GetString("StaffCode");          

            if (string.IsNullOrEmpty(staffCode))
            {
                return RedirectToAction("Login","LoginUsers");
            }

            var model = new ChangePasswordViewModel
            {
                StaffCode = staffCode,
            };

            return View("~/Views/Password/ChangePassword.cshtml", model);
        }

        [HttpPost("ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var staffCode = HttpContext.Session.GetString("StaffCode");
            if (string.IsNullOrEmpty(staffCode))
            {
                return RedirectToAction("Login", "LoginUsers");
            }

            model.StaffCode = staffCode;
            if (!ModelState.IsValid)
            {
                return View("~/Views/Password/ChangePassword.cshtml", model);
            }
            int result = await _changePasswordBL.ChangePasswordAsync(model);

            switch (result)
            {
                case 0:
                    TempData["SuccessMessage"] = "Password changed successfully";
                    return RedirectToAction("ChangePassword");

                case 1:
                    ModelState.AddModelError(
                        "",
                        "User was not found.");
                    break;

                case 2:
                    ModelState.AddModelError(
                        "CurrentPassword",
                        "Current password is incorrect");
                    break;

                case 3:
                    ModelState.AddModelError(
                        "",
                        "Invalid password information.");
                    break;
                case 4:
                    ModelState.AddModelError(
                        "NewPassword",
                        "New password must be different from the current password.");
                    break;
                default:
                      TempData["ErrorMessage"] =
                      "Failed to change password";
                     return RedirectToAction("Entry", "departments");
            }
            return View("~/Views/Password/ChangePassword.cshtml", model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");

            return RedirectToAction("Login","LoginUsers");
        }
    }
}

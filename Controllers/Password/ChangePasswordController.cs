using CKM_ManagementSystem.Models.ViewModels.Password;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.BL;

namespace CKM_ManagementSystem.Controllers.Password
{
   // [Authorize]
    [Route("Password")]
    public class ChangePasswordController : Controller
    {
        private readonly ChangePasswordBL _changePasswordBL;

        public ChangePasswordController(ChangePasswordBL changePasswordBL)
        {
            _changePasswordBL = changePasswordBL;
        }
        /*
        [HttpGet("TestBL")]
        public async Task<IActionResult> TestBL()
        {
            var model = new ChangePasswordViewModel
            {
                StaffCode = "CKM-0041",
                CurrentPassword = "TestUser41-1",
                NewPassword = "TestUser41",
                ConfirmPassword = "TestUser41-1"
            };

            int result = await _changePasswordBL.ChangePasswordAsync(model);

            return Content($"Result Code: {result}");
        }  */
        
        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            HttpContext.Session.SetString("StaffCode", "CKM-0042"); // Test code *///// if want change password , TestUserxx-1 or TestUserxx (TestUser42)
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

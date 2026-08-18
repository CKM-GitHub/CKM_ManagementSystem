using CKM_ManagementSystem.BL.Interface;
using CKM_ManagementSystem.Models.ViewModels.Password;
using Microsoft.AspNetCore.Mvc;

namespace CKM_ManagementSystem.Controllers.Password
{
    [Route("Password")]
    public class ChangePasswordController : Controller
    {
        private readonly IChangePasswordBL _changePasswordBL;

        public ChangePasswordController(IChangePasswordBL changePasswordBL)
        {
            _changePasswordBL = changePasswordBL;
        }
        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword(string StaffCode)
        {
            var model = new ChangePasswordViewModel
            {
                StaffCode = StaffCode
            };
            return View("~/Views/Password/ChangePassword.cshtml", model);
        }

        [HttpPost("ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Password/ChangePassword.cshtml", model);
            }
            int result = await _changePasswordBL.ChangePasswordAsync(model);

            switch (result)
            {
                case 0:
                    TempData["SuccessMessage"] = "Password changed successfully";
                    return RedirectToAction(
                        "ChangePassword",
                        new { StaffCode = model.StaffCode });

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

                default:
                      TempData["ErrorMessage"] =
                      "Failed to change password";
                     return RedirectToAction("Entry", "departments");
            }
            return View("~/Views/Password/ChangePassword.cshtml", model);

        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CKM_ManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult DebugSession()
        {
            var json = HttpContext.Session.GetString("UserPermissions");
            var staffCode = HttpContext.Session.GetString("staffCode");

            return Json(new
            {
                StaffCode = staffCode,
                HasPermissions = !string.IsNullOrEmpty(json),
                PermissionsJson = json,
                IsAuthenticated = User.Identity?.IsAuthenticated,
                UserName = User.Identity?.Name
            });
        }
    }
}

using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CKM_ManagementSystem.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        private const string AuthScheme = "MyCookieAuth";

        [Route("Error/StatusCode/{code:int}")]
        public async Task<IActionResult> Error(int code)
        {
            var model = new ErrorViewModel
            {
                ErrorCode = code,
                RedirectUrl = Url.Action("Entry", "DepartmentEntry") ?? "/",
                ButtonText = "Back to Department"
            };

            switch (code)
            {
                case 400:
                    model.Title = "Bad Request";
                    model.Message = "The request could not be processed.";
                    break;

                case 401:
                    model.Title = "Unauthorized";
                    model.Message = "Your session has expired or you need to log in.";
                    model.ButtonText = "Go to Login";
                    model.RedirectUrl = Url.Action("Login", "LoginUsers") ?? "/";

                    await HttpContext.SignOutAsync(AuthScheme);
                    HttpContext.Session.Clear();  
                    break;

                case 403:
                    model.Title = "Access Denied";
                    model.Message = "出て行って !!";
                    var perms = HttpContext.Session.GetString("UserPermissions");
                    if (string.IsNullOrEmpty(perms))
                    {
                        model.Message = "Your session has expired. Please log in again.";
                        model.ButtonText = "Go to Login";
                        model.RedirectUrl = Url.Action("Login", "LoginUsers") ?? "/LoginUsers/Login";

                        await HttpContext.SignOutAsync("MyCookieAuth");
                        HttpContext.Session.Clear();
                    }
                    else
                    {
                        model.ButtonText = "Back to Department";
                        model.RedirectUrl = Url.Action("Entry", "DepartmentEntry") ?? "/";
                    }            
                    break;

                case 404:
                    model.Title = "Page Not Found";
                    model.Message = "寺へ行って、寺はそちらね.";
                    model.ButtonText = "Back to Department";
                    model.RedirectUrl = Url.Action("Entry", "DepartmentEntry") ?? "/";
                    break;

                case 500:
                    model.Title = "Internal Server Error";
                    model.Message = "Something went wrong. Please try again later.";
                    model.ButtonText = "Back to Department";
                    model.RedirectUrl = Url.Action("Entry", "DepartmentEntry") ?? "/";
                    break;

                default:
                    model.ErrorCode = 500;
                    model.Title = "Unexpected Error";
                    model.Message = "An unexpected error occurred.";
                    model.ButtonText = "Back to Department";
                    model.RedirectUrl = Url.Action("Entry", "DepartmentEntry") ?? "/";
                    code = 500;
                    break;
            }

            Response.StatusCode = code;

            return View("Error", model);
        }
    }
}
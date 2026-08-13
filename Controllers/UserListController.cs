using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using CKM_ManagementSystem.BL.Interface;

namespace CKM_ManagementSystem.Controllers
{
    public class UserListController : Controller
    {
        private readonly IUserListBL _userListBL;
        public UserListController(IUserListBL userListBL)
        {
            _userListBL = userListBL;
        }
        public async Task<IActionResult> UserList (
            string? searchText,
            bool? status,
            string? departmentCode,
            string? roleCode,
            int pageNumber = 1,
            int pageSize = 10)
        {
            ViewBag.SearchText = searchText;
            ViewBag.Status = status;
            ViewBag.DepartmentCode = departmentCode;
            ViewBag.RoleCode = roleCode;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            var model = await _userListBL.GetUserListAsync(
                searchText,
                status,
                departmentCode,
                roleCode,
                pageNumber,
                pageSize
                );

            if (model.ErrorCode != 0)
            {
                ModelState.AddModelError(string.Empty, "Error Shi Dl !!");
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string staffCode)
        {
            if (string.IsNullOrWhiteSpace(staffCode))
            {
                TempData["ErrorMessage"] = "Sraff Code is Required!!";

                return RedirectToAction("UserList");
            }
            
            var result = await _userListBL.DeleteUserAsync(staffCode);
            if (result.ErrorCode == 0)
            {
                   TempData["SuccessMessage"] = $"User Delete successfully! User : {result.UserName}  ";
            }
            else
            {
                   TempData["ErrorMessage"] = "User could not be deleted";
            }

            return RedirectToAction("UserList");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.BL;
using System.Reflection.Metadata.Ecma335;

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
           // ViewBag.RoleList = await _userListBL.GetRoleListAsync(); 
           // ViewBag.DepartmentList = await _userListBL.GetDepartmentListAsync();

            var model = await _userListBL.GetUserListAsync(
                searchText,
                status,
                departmentCode,
                roleCode,
                pageNumber,
                pageSize
                );

            return View(model);
        }
     //   [HttpPost]
    //    public async Task<IActionResult> DeleteUser(string staffCode)
     //   {


      //      return RedirectToAction("UserList");
     //   }
    }
}

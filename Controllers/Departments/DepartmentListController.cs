using CKM_ManagementSystem.Authorization;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.BL.Permissions;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Departments;
using CKM_ManagementSystem.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CKM_ManagementSystem.Controllers.Departments
{
    public class DepartmentListController : Controller
    {
        private readonly DepartmentBL _departmentBL;
        private readonly CurrentUserPermission _permission;        

        public DepartmentListController(DepartmentBL departmentBL, CurrentUserPermission permission)
        {
            _departmentBL = departmentBL;
            _permission = permission;
        }

        [Authorize(Policy = "Permission.Department.Read")]
        [HttpGet]
        public IActionResult Index(
            string? searchText,
            bool? status,
            int pageNumber = 1)
        {
            DepartmentListViewModel viewModel =
                _departmentBL.GetDepartmentList(
                    searchText,
                    status,
                    pageNumber,
                    10);

            viewModel.CanWrite = _permission.CanWrite(MenuIDs.Department);
            viewModel.CanDelete = _permission.CanDelete(MenuIDs.Department);

            return View(
                "~/Views/Departments/List.cshtml",
                viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "Permission.Department.Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(
            string departmentCode)
        {
            if (string.IsNullOrWhiteSpace(
                departmentCode))
            {
                TempData["ErrorMessage"] =
                    "Department code is required.";

                return RedirectToAction(nameof(Index));
            }

            string result =
                _departmentBL.DeleteDepartment(
                    departmentCode);

            if (result == "true")
            {
                TempData["SuccessMessage"] =
                    "Department deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    "Department deletion failed.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
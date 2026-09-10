using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels.Departments;
using CKM_ManagementSystem.Models.Entities;
namespace CKM_ManagementSystem.Controllers.Departments
{
    public class DepartmentListController : Controller
    {
        private readonly DepartmentBL _departmentBL;

        public DepartmentListController(DepartmentBL departmentBL)
        {
            _departmentBL = departmentBL;
        }

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

            return View(
                "~/Views/Departments/List.cshtml",
                viewModel);
        }

        [HttpPost]
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
using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Departments;

namespace CKM_ManagementSystem.Controllers.Departments
{
    public class DepartmentsController : Controller
    {
        private readonly DepartmentBL _departmentBL;

        public DepartmentsController(DepartmentBL departmentBL)
        {
            _departmentBL = departmentBL;
        }

        [HttpGet]
        public IActionResult Entry()
        {
            return View(new DepartmentEntryViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Entry(DepartmentEntryViewModel model)
        {
            model.DepartmentCode =
                model.DepartmentCode?.Trim()
                ?? string.Empty;

            model.DepartmentName =
                model.DepartmentName?.Trim()
                ?? string.Empty;

            model.Description =
                model.Description?.Trim();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool exists =
                _departmentBL.IsDepartmentCodeDuplicate(
                    model.DepartmentCode);

            if (exists)
            {
                ModelState.AddModelError(
                    nameof(model.DepartmentCode),
                    "Department Code already exists.");

                return View(model);
            }

            bool nameExists =
                _departmentBL.IsDepartmentNameDuplicate(
                    model.DepartmentName);

            if (nameExists)
            {
                ModelState.AddModelError(
                    nameof(model.DepartmentName),
                    "Department Name already exists.");

                return View(model);
            }

            Department department = new Department
            {
                DepartmentCode = model.DepartmentCode,
                DepartmentName = model.DepartmentName,
                Description = model.Description,
                Status = model.Status
            };

            string result =
                _departmentBL.Department_Insert(department);

            if (result != "true")
            {
                ModelState.AddModelError(
                    "",
                    "Save failed.");

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Registration is complete.";

            return RedirectToAction(nameof(Index));
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

            return View("List", viewModel);
        }

        [HttpGet]
        public IActionResult Edit(string departmentCode)
        {
            if (string.IsNullOrWhiteSpace(departmentCode))
            {
                TempData["ErrorMessage"] =
                    "Department code is required.";

                return RedirectToAction(nameof(Index));
            }

            DepartmentEntryViewModel? viewModel =
                _departmentBL.GetDepartmentByCode(
                    departmentCode);

            if (viewModel == null)
            {
                TempData["ErrorMessage"] =
                    "Department was not found.";

                return RedirectToAction(nameof(Index));
            }

            return View("Entry", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            DepartmentEntryViewModel model)
        {
            if (string.IsNullOrWhiteSpace(
                model.OriginalDepartmentCode))
            {
                TempData["ErrorMessage"] =
                    "Department code is required.";

                return RedirectToAction(nameof(Index));
            }

            model.DepartmentName =
                model.DepartmentName?.Trim()
                ?? string.Empty;

            model.Description =
                model.Description?.Trim();

            if (!ModelState.IsValid)
            {
                return View("Entry", model);
            }

            bool nameExists =
                _departmentBL.IsDepartmentNameDuplicateForUpdate(
                    model.DepartmentName,
                    model.OriginalDepartmentCode);

            if (nameExists)
            {
                ModelState.AddModelError(
                    nameof(model.DepartmentName),
                    "Department Name already exists.");

                return View("Entry", model);
            }

            Department department = new Department
            {
                OriginalDepartmentCode =
                    model.OriginalDepartmentCode,

                DepartmentCode =
                    model.OriginalDepartmentCode,

                DepartmentName =
                    model.DepartmentName,

                Description =
                    model.Description,

                Status =
                    model.Status
            };

            string result =
                _departmentBL.Department_Update(department);

            if (result != "true")
            {
                ModelState.AddModelError(
                    "",
                    "Update failed.");

                return View("Entry", model);
            }

            TempData["SuccessMessage"] =
                "Department updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string departmentCode)
        {
            if (string.IsNullOrWhiteSpace(departmentCode))
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
using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Departments;

namespace CKM_ManagementSystem.Controllers.Departments
{
    public class DepartmentEntryController : Controller
    {
        private readonly DepartmentBL _departmentBL;

        public DepartmentEntryController(DepartmentBL departmentBL)
        {
            _departmentBL = departmentBL;
        }

        [HttpGet]
        public IActionResult Entry()
        {
            return View(
                "~/Views/Departments/Entry.cshtml",
                new DepartmentEntryViewModel());
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
                return View(
                    "~/Views/Departments/Entry.cshtml",
                    model);
            }

            bool exists =
                _departmentBL.IsDepartmentCodeDuplicate(
                    model.DepartmentCode);

            if (exists)
            {
                ModelState.AddModelError(
                    nameof(model.DepartmentCode),
                    "Department Code already exists.");

                return View(
                    "~/Views/Departments/Entry.cshtml",
                    model);
            }

            bool nameExists =
                _departmentBL.IsDepartmentNameDuplicate(
                    model.DepartmentName);

            if (nameExists)
            {
                ModelState.AddModelError(
                    nameof(model.DepartmentName),
                    "Department Name already exists.");

                return View(
                    "~/Views/Departments/Entry.cshtml",
                    model);
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

                return View(
                    "~/Views/Departments/Entry.cshtml",
                    model);
            }

            TempData["SuccessMessage"] =
                "Registration is complete.";

            return RedirectToAction(nameof(Entry));
        }

    }
}
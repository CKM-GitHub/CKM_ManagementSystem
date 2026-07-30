using CKM_ManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Departments;

namespace CKM_ManagementSystem.Controllers.Departments
{
    public class DepartmentController : Controller
    {
        private readonly DepartmentDL _departmentDL;

        public DepartmentController(DepartmentDL departmentDL)
        {
            _departmentDL = departmentDL;
        }


        //Get Method
        [HttpGet]
        public IActionResult Entry()
        { 
          return View(new DepartmentEntryViewModel());
        }



        //Post Method
        [HttpPost]
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


            bool exists = _departmentDL
                .IsDepartmentCodeDuplicate(model.DepartmentCode);
            if (exists)
            {
                ModelState.AddModelError(
                    nameof(model.DepartmentCode),
                    "Department Code already exists.");

                return View(model);


            }
            bool nameExists = _departmentDL
                .IsDepartmentNameDuplicate(model.DepartmentName);

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

            string result = _departmentDL.Department_Insert(department);

            if (result != "true")
            {
                ModelState.AddModelError("", "Save failed.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Registration is complete.";

            return RedirectToAction(nameof(Entry));
        }

    }
}
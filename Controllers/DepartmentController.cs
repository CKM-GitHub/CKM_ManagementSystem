using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CKM_ManagementSystem.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }


        //Get Method
        [HttpGet]
        public IActionResult Entry()
        { 
          return View(new DepartmentEntryViewModel());
        }



        //Post Method
        [HttpPost]
        public async Task<IActionResult> Entry(DepartmentEntryViewModel model)
        {
                 model.DepartmentCode =
                 model.DepartmentCode?.Trim().ToUpperInvariant()
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

            bool exists = await _departmentService
                    .IsDepartmentCodeDuplicateAsync(model.DepartmentCode);
            if (exists)
            {
                ModelState.AddModelError(
                    nameof(model.DepartmentCode),
                    "Department Code already exists.");

                return View(model);


            }

           bool nameExists = await _departmentService
                         .IsDepartmentNameDuplicateAsync(
                        model.DepartmentName);
             
            if (nameExists)
            {
                ModelState.AddModelError(
                    nameof(model.DepartmentName),
                    "Department Name already exists.");

                return View(model);
            }
            await _departmentService.CreateAsync(model);
            return RedirectToAction(nameof(Entry));
        }

    }
}
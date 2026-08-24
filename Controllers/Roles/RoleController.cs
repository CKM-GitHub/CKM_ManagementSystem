using System;
using System.Linq;
using System.Threading.Tasks;
using CKM_ManagementSystem.Models.ViewModels.Roles;
using CKM_ManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CKM_ManagementSystem.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> RoleEntry(string? roleCode)
        {
            var model = new RoleEntryViewModel();

            if (!string.IsNullOrEmpty(roleCode))
            {
                model.RoleCode = roleCode;
                model.MenuPermissions = await _roleService.GetMenuPermissionsAsync(roleCode);
            }
            else
            {
                model.MenuPermissions = await _roleService.GetMenuPermissionsAsync();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CheckDuplicateCode(string roleCode)
        {
            bool isDuplicate = await _roleService.CheckDuplicateRoleCodeAsync(roleCode);
            return Json(new { isDuplicate = isDuplicate });
        }

        [HttpPost]
        public async Task<IActionResult> SaveRole(RoleEntryViewModel model, bool isEdit = false)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .FirstOrDefault();
                return Json(new { success = false, message = errors ?? "Validation failed." });
            }

            try
            {
                await _roleService.SaveRoleWithPermissionsAsync(model);

                return Json(new
                {
                    success = true,
                    isEdit = isEdit,
                    message = "Registration is complete."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while saving data: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult RoleList()
        {
            return View();
        }
    }
}
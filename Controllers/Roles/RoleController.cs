using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CKM_ManagementSystem.Models.ViewModels.Roles;
using CKM_ManagementSystem.Services.Interfaces;

namespace CKM_ManagementSystem.Controllers.Roles
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
            }

            model.MenuPermissions = await _roleService.GetMenuPermissionsAsync(roleCode);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRole([FromBody] RoleEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data submitted." });
            }

            try
            {
                await _roleService.SaveRoleWithPermissionsAsync(model);
                return Json(new { success = true, message = "Registration is complete." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while saving data: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> RoleList(int pageNumber = 1, int pageSize = 10, string? searchKeyword = null, int? status = null)
        {
            var model = await _roleService.GetRoleListPagedAsync(pageNumber, pageSize, searchKeyword, status);
            return View(model);
        }
    }
}
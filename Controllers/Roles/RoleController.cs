using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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
        public async Task<IActionResult> RoleEntry(string? code)
        {
            var model = new RoleEntryViewModel();
            bool isEdit = !string.IsNullOrEmpty(code);

            if (isEdit)
            {
                var existingRole = await _roleService.GetRoleByCodeAsync(code);
                if (existingRole == null)
                {
                    return NotFound();
                }
                model = existingRole;
            }

            ViewBag.IsEdit = isEdit;
            model.MenuPermissions = await _roleService.GetMenuPermissionsAsync(code);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRole([FromForm] RoleEntryViewModel model, [FromQuery] bool isEdit = false)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, message = "Validation Error: " + string.Join(", ", errors) });
            }

            try
            {
                if (!isEdit)
                {
                    bool isDuplicate = await _roleService.CheckDuplicateRoleCodeAsync(model.RoleCode);
                    if (isDuplicate)
                    {
                        return Json(new { success = false, message = "This Role Code already exists." });
                    }
                }

                await _roleService.SaveRoleWithPermissionsAsync(model);
                return Json(new { success = true, isEdit = isEdit, message = isEdit ? "Update is complete." : "Registration is complete." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while saving data: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckDuplicateCode(string roleCode)
        {
            if (string.IsNullOrWhiteSpace(roleCode))
            {
                return Json(new { isDuplicate = false });
            }

            bool isDuplicate = await _roleService.CheckDuplicateRoleCodeAsync(roleCode);
            return Json(new { isDuplicate = isDuplicate });
        }

        [HttpGet]
        public async Task<IActionResult> RoleList(int pageNumber = 1, int pageSize = 10, string? searchKeyword = null, int? status = null)
        {
            var model = await _roleService.GetRoleListPagedAsync(pageNumber, pageSize, searchKeyword, status);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleCode)
        {
            if (string.IsNullOrEmpty(roleCode))
            {
                return Json(new { success = false, message = "Invalid Role Code." });
            }

            try
            {
                var result = await _roleService.DeleteRoleAsync(roleCode);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while deleting: " + ex.Message });
            }
        }
    }
}
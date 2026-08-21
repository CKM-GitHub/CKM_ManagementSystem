using System;
using System.Collections.Generic;
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
        public async Task<IActionResult> RoleList(int pageNumber = 1, int pageSize = 10, string searchKeyword = "", int? status = null)
        {
            var model = await _roleService.GetRoleListPagedAsync(pageNumber, pageSize, searchKeyword, status);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RoleEntry(string? code = null)
        {
            var model = new RoleEntryViewModel();
            bool isEdit = !string.IsNullOrEmpty(code);
            ViewBag.IsEdit = isEdit;

            if (isEdit)
            {
                model = await _roleService.GetRoleByCodeSPAsync(code);
                if (model == null)
                {
                    return NotFound();
                }
                model.MenuPermissions = await _roleService.GetMenuPermissionsAsync(code);
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
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = string.Join(" ", errors) });
            }

            if (!isEdit)
            {
                bool isDuplicate = await _roleService.CheckDuplicateRoleCodeAsync(model.RoleCode);
                if (isDuplicate)
                {
                    return Json(new { success = false, message = "This Role Code already exists." });
                }
            }
            else
            {
                var existingRole = await _roleService.GetRoleByCodeSPAsync(model.RoleCode);
                if (existingRole != null)
                {
                    var existingPermissions = await _roleService.GetMenuPermissionsAsync(model.RoleCode);
                    bool isDataChanged = IsRoleDataChanged(existingRole, existingPermissions, model);

                    if (!isDataChanged)
                    {
                        
                        return Json(new { success = true, message = "No changes were made." });
                    }
                }
            }

            try
            {
                await _roleService.SaveRoleWithPermissionsAsync(model);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Data save error: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string roleCode)
        {
            if (string.IsNullOrEmpty(roleCode))
            {
                return Json(new { success = false, message = "Role Code is missing." });
            }

            try
            {
                var (success, message) = await _roleService.DeleteRoleAsync(roleCode);
                return Json(new { success = success, message = message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "System error: " + ex.Message });
            }
        }

        private bool IsRoleDataChanged(RoleEntryViewModel existing, List<MenuPermissionViewModel> existingPermissions, RoleEntryViewModel newModel)
        {
            if ((existing.DisplayName ?? "").Trim() != (newModel.DisplayName ?? "").Trim()) return true;
            if ((existing.Description ?? "").Trim() != (newModel.Description ?? "").Trim()) return true;
            if (existing.Status != newModel.Status) return true;

            if (existingPermissions != null && newModel.MenuPermissions != null)
            {
                foreach (var newPerm in newModel.MenuPermissions)
                {
                    var oldPerm = existingPermissions.FirstOrDefault(p => p.MenuId == newPerm.MenuId);
                    if (oldPerm != null)
                    {
                        if (oldPerm.CanRead != newPerm.CanRead ||
                            oldPerm.CanWrite != newPerm.CanWrite ||
                            oldPerm.CanDelete != newPerm.CanDelete)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKM_ManagementSystem.Models.ViewModels;
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
        public async Task<IActionResult> RoleEntry(string code = null)
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
            ViewBag.IsEdit = isEdit;

            if (!ModelState.IsValid)
            {
                if (model.MenuPermissions == null || !model.MenuPermissions.Any())
                {
                    model.MenuPermissions = await _roleService.GetMenuPermissionsAsync(isEdit ? model.RoleCode : null);
                }
                return View("RoleEntry", model);
            }

            if (!isEdit)
            {
                bool isDuplicate = await _roleService.CheckDuplicateRoleCodeAsync(model.RoleCode);
                if (isDuplicate)
                {
                    ModelState.AddModelError("RoleCode", "ဒီ Role Code မှာ ရှိပြီးသား ဖြစ်နေပါသည်။");
                    if (model.MenuPermissions == null || !model.MenuPermissions.Any())
                    {
                        model.MenuPermissions = await _roleService.GetMenuPermissionsAsync();
                    }
                    return View("RoleEntry", model);
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
                        return RedirectToAction("RoleList");
                    }
                }
            }

            try
            {
                await _roleService.SaveRoleWithPermissionsAsync(model);

                TempData["SuccessMessage"] = isEdit
                    ? "Role အချက်အလက်များကို အောင်မြင်စွာ ပြင်ဆင်ပြီးပါပြီ။"
                    : "Role သစ်ကို အောင်မြင်စွာ သိမ်းဆည်းပြီးပါပြီ။";

                return RedirectToAction("RoleList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Data သိမ်းဆည်းရာတွင် အမှားအယွင်း ရှိနေပါသည်: " + ex.Message);
                if (model.MenuPermissions == null || !model.MenuPermissions.Any())
                {
                    model.MenuPermissions = await _roleService.GetMenuPermissionsAsync(isEdit ? model.RoleCode : null);
                }
                return View("RoleEntry", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string roleCode)
        {
            if (string.IsNullOrEmpty(roleCode))
            {
                return Json(new { success = false, message = "Role Code မရှိပါ။" });
            }

            try
            {
                var (success, message) = await _roleService.DeleteRoleAsync(roleCode);
                return Json(new { success = success, message = message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "စနစ်အတွင်း အမှားအယွင်း ဖြစ်ပေါ်နေပါသည်: " + ex.Message });
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
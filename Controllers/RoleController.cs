using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKM_ManagementSystem.Models.ViewModels;
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
        public async Task<IActionResult> RoleList(string searchKeyword = "", int? status = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                
                int? filterStatus = status;

                
                var pagedData = await _roleService.GetRoleListAsync(searchKeyword, filterStatus, pageNumber, pageSize);

                return View(pagedData);
            }
            catch (NotImplementedException)
            {
                return View(new RoleListPagedViewModel());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(new RoleListPagedViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> RoleEntry(string code = null)
        {
            var model = new RoleEntryViewModel();

            if (!string.IsNullOrEmpty(code))
            {
                model = await _roleService.GetRoleByCodeAsync(code);

                if (model == null)
                {
                    TempData["ErrorMessage"] = "သက်ဆိုင်ရာ Role Data ရှာမတွေ့ပါ။";
                    return RedirectToAction("RoleList");
                }
            }

            if (model.MenuPermissions == null || !model.MenuPermissions.Any())
            {
                model.MenuPermissions = GetDefaultMenuPermissions();
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
                if (model.MenuPermissions == null || !model.MenuPermissions.Any())
                {
                    model.MenuPermissions = GetDefaultMenuPermissions();
                }
                return View("RoleEntry", model);
            }

            if (isEdit)
            {
                var existingRole = await _roleService.GetRoleByCodeAsync(model.RoleCode);

                if (existingRole != null)
                {
                    bool isDataChanged = existingRole.DisplayName != model.DisplayName ||
                                         existingRole.Description != model.Description ||
                                         existingRole.Status != model.Status;

                    if (!isDataChanged)
                    {
                        return RedirectToAction("RoleList");
                    }
                }
            }
            else
            {
                bool isDuplicate = await _roleService.CheckDuplicateRoleCodeAsync(model.RoleCode);
                if (isDuplicate)
                {
                    ModelState.AddModelError("RoleCode", "ဒီ Role Code မှာ ရှိပြီးသား ဖြစ်နေပါသည်။");
                    return View("RoleEntry", model);
                }
            }

            try
            {
                await _roleService.SaveRoleWithPermissionsAsync(model);

                TempData["SuccessMessage"] = isEdit
                    ? "Role ကို အောင်မြင်စွာ ပြင်ဆင်ပြီးပါပြီ။"
                    : "Role သစ်ကို အောင်မြင်စွာ သိမ်းဆည်းပြီးပါပြီ။";

                return RedirectToAction("RoleList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Data သိမ်းဆည်းရာတွင် အမှားအယွင်း ရှိနေပါသည်: " + ex.Message);
                return View("RoleEntry", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleCode)
        {
            try
            {
                if (string.IsNullOrEmpty(roleCode))
                {
                    return Json(new { success = false, message = "Role Code မရှိပါ။" });
                }

                var role = await _roleService.GetRoleByCodeAsync(roleCode);
                if (role == null)
                {
                    return Json(new { success = false, message = "ဖျက်လိုသော Role ကို ရှာမတွေ့ပါ။" });
                }

                if (role.Status)
                {
                    return Json(new { success = false, message = "Active ဖြစ်နေသော Role ကို ဖျက်၍ မရပါ။ Inactive ပြောင်းပြီးမှ ဖျက်ပါ။" });
                }

                bool isDeleted = await _roleService.DeleteRoleAsync(roleCode);

                if (isDeleted)
                {
                    return Json(new { success = true, message = "Role ကို အောင်မြင်စွာ ဖျက်ပြီးပါပြီ။" });
                }
                else
                {
                    return Json(new { success = false, message = "Role ကို ဖျက်၍ မရပါ။" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ဖျက်ရာတွင် အမှားအယွင်း ရှိနေပါသည်: " + ex.Message });
            }
        }

        private List<MenuPermissionViewModel> GetDefaultMenuPermissions()
        {
            return new List<MenuPermissionViewModel>
            {
                new MenuPermissionViewModel { MenuId = 1, MenuName = "Main Menu 1", ParentId = null },
                new MenuPermissionViewModel { MenuId = 2, MenuName = "Submenu 1", ParentId = 1 },
                new MenuPermissionViewModel { MenuId = 3, MenuName = "Submenu 2", ParentId = 1 },
                new MenuPermissionViewModel { MenuId = 4, MenuName = "Submenu 3", ParentId = 1 },

                new MenuPermissionViewModel { MenuId = 5, MenuName = "Main Menu 2", ParentId = null },
                new MenuPermissionViewModel { MenuId = 6, MenuName = "Submenu 1", ParentId = 5 },
                new MenuPermissionViewModel { MenuId = 7, MenuName = "Submenu 2", ParentId = 5 },
                new MenuPermissionViewModel { MenuId = 8, MenuName = "Submenu 3", ParentId = 5 },

                new MenuPermissionViewModel { MenuId = 9, MenuName = "Main Menu 3", ParentId = null },
                new MenuPermissionViewModel { MenuId = 10, MenuName = "Submenu 1", ParentId = 9 },
                new MenuPermissionViewModel { MenuId = 11, MenuName = "Submenu 2", ParentId = 9 },

                new MenuPermissionViewModel { MenuId = 12, MenuName = "Main Menu 4", ParentId = null }
            };
        }
    }
}
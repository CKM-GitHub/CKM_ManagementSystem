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

        #region Role List & Delete Actions

        [HttpGet]
        public async Task<IActionResult> RoleList(string searchKeyword, int? status)
        {
            ViewBag.SearchKeyword = searchKeyword;
            ViewBag.Status = status;

            var roleList = await _roleService.GetRoleListAsync(searchKeyword, status);
            return View(roleList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string roleCode)
        {
            try
            {
                if (string.IsNullOrEmpty(roleCode))
                {
                    return Json(new { success = false, message = "Role Code သတ်မှတ်ထားခြင်း မရှိပါ။" });
                }

                var roleListPaged = await _roleService.GetRoleListAsync(roleCode, null);
                var currentRole = roleListPaged.Roles?.FirstOrDefault(r => r.RoleCode == roleCode);

                if (currentRole != null && currentRole.Status)
                {
                    return Json(new
                    {
                        success = false,
                        message = "ဒီ Role က Active ဖြစ်နေပါသေးသည်။ Active ဖြစ်နေသော Role များကို ဖျက်၍မရပါ (Inactive ပြုလုပ်ပြီးမှ ဖျက်ပါ)။"
                    });
                }

                bool isSuccess = await _roleService.DeleteRoleAsync(roleCode);

                if (isSuccess)
                {
                    return Json(new { success = true, message = "Role ကို အောင်မြင်စွာ ဖျက်ပြီးပါပြီ။" });
                }
                else
                {
                    return Json(new { success = false, message = "ဒီ Role ကို ဖျက်၍ မရနိုင်ပါ။" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "စနစ်အတွင်း အမှားအယွင်း ဖြစ်ပေါ်နေပါသည်: " + ex.Message });
            }
        }

        #endregion

        #region Role Entry Actions (Add & Edit)

        [HttpGet]
        public async Task<IActionResult> RoleEntry(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var roleListPaged = await _roleService.GetRoleListAsync(code, null);
                var existingRole = roleListPaged.Roles?.FirstOrDefault(r => r.RoleCode == code);

                if (existingRole != null)
                {
                    var editModel = new RoleEntryViewModel
                    {
                        RoleCode = existingRole.RoleCode,
                        DisplayName = existingRole.DisplayName,
                        Description = existingRole.Description,
                        Status = existingRole.Status,
                        MenuPermissions = existingRole.MenuPermissions ?? new List<MenuPermissionViewModel>()
                    };

                    ViewBag.IsEditMode = true;
                    return View(editModel);
                }
            }

            var model = new RoleEntryViewModel
            {
                MenuPermissions = new List<MenuPermissionViewModel>()
            };

            ViewBag.IsEditMode = false;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CheckDuplicateCode(string roleCode)
        {
            bool isDuplicate = await _roleService.CheckDuplicateRoleCodeAsync(roleCode);
            return Json(new { isDuplicate = isDuplicate });
        }

        [HttpPost]
        public async Task<IActionResult> SaveRole(RoleEntryViewModel model)
        {
            bool isEditMode = await _roleService.CheckDuplicateRoleCodeAsync(model.RoleCode);

            if (!ModelState.IsValid)
            {
                ViewBag.IsEditMode = isEditMode;
                return View("RoleEntry", model);
            }

            try
            {
                if (isEditMode)
                {
               
                    var roleListPaged = await _roleService.GetRoleListAsync(model.RoleCode, null);
                    var existingRole = roleListPaged.Roles?.FirstOrDefault(r => r.RoleCode == model.RoleCode);

                    if (existingRole != null)
                    {
                        
                        bool isPermissionsChanged = false;
                        var existingPerms = existingRole.MenuPermissions ?? new List<MenuPermissionViewModel>();
                        var modelPerms = model.MenuPermissions ?? new List<MenuPermissionViewModel>();

                        if (existingPerms.Count != modelPerms.Count)
                        {
                            isPermissionsChanged = true;
                        }
                        else
                        {
                            foreach (var p in modelPerms)
                            {
                                var oldP = existingPerms.FirstOrDefault(x => x.MenuId == p.MenuId);
                                if (oldP == null || oldP.CanRead != p.CanRead || oldP.CanWrite != p.CanWrite || oldP.CanDelete != p.CanDelete)
                                {
                                    isPermissionsChanged = true;
                                    break;
                                }
                            }
                        }

                        
                        bool isChanged = (existingRole.DisplayName != model.DisplayName) ||
                                         (existingRole.Description != model.Description) ||
                                         (existingRole.Status != model.Status) ||
                                         isPermissionsChanged;

                        if (!isChanged)
                        {
                            
                            return RedirectToAction("RoleList");
                        }
                    }
                }

               
                await _roleService.SaveRoleWithPermissionsAsync(model);

                if (isEditMode)
                {
                    TempData["SuccessMessage"] = "Role အချက်အလက်များကို အောင်မြင်စွာ ပြင်ဆင်ပြီးပါပြီ။";
                    return RedirectToAction("RoleList");
                }
                else
                {
                    TempData["SuccessMessage"] = "Role အသစ်ကို အောင်မြင်စွာ သိမ်းဆည်းပြီးပါပြီ။";
                    return RedirectToAction("RoleEntry");
                }
            }
            catch (Exception ex)
            {
                ViewBag.IsEditMode = isEditMode;
                ModelState.AddModelError("", "Data သိမ်းဆည်းရာတွင် အမှားအယွင်း ရှိနေပါသည်: " + ex.Message);
                return View("RoleEntry", model);
            }
        }

        #endregion
    }
}
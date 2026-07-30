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
        public IActionResult RoleEntry()
        {
            var model = new RoleEntryViewModel
            {
                MenuPermissions = new List<MenuPermissionViewModel>
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
                }
            };

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
            
            if (!ModelState.IsValid)
            {
                return View("RoleEntry", model);
            }

            
            bool isDuplicate = await _roleService.CheckDuplicateRoleCodeAsync(model.RoleCode);
            if (isDuplicate)
            {
                ModelState.AddModelError("RoleCode", "ဒီ Role Code မှာ ရှိပြီးသား ဖြစ်နေပါသည်။");
                return View("RoleEntry", model);
            }

            try
            {
                await _roleService.SaveRoleWithPermissionsAsync(model);

                
                TempData["SuccessMessage"] = "Role သစ်ကို အောင်မြင်စွာ သိမ်းဆည်းပြီးပါပြီ။";
                return RedirectToAction("RoleList"); 
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Data သိမ်းဆည်းရာတွင် အမှားအယွင်း ရှိနေပါသည်: " + ex.Message);
                return View("RoleEntry", model);
            }
        }
    }
}
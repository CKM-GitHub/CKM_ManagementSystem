using System;
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
        public async Task<IActionResult> RoleEntry()
        {
            var model = new RoleEntryViewModel
            {
                MenuPermissions = await _roleService.GetMenuPermissionsAsync()
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
                
                if (model.MenuPermissions == null || !model.MenuPermissions.Any())
                {
                    model.MenuPermissions = await _roleService.GetMenuPermissionsAsync();
                }
                return View("RoleEntry", model);
            }

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

            try
            {
                await _roleService.SaveRoleWithPermissionsAsync(model);

                TempData["SuccessMessage"] = "Role သစ်ကို အောင်မြင်စွာ သိမ်းဆည်းပြီးပါပြီ။";
                return RedirectToAction("RoleList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Data သိမ်းဆည်းရာတွင် အမှားအယွင်း ရှိနေပါသည်: " + ex.Message);
                if (model.MenuPermissions == null || !model.MenuPermissions.Any())
                {
                    model.MenuPermissions = await _roleService.GetMenuPermissionsAsync();
                }
                return View("RoleEntry", model);
            }
        }
    }
}
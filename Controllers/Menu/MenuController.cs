using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CKM_ManagementSystem.Controllers.Menu
{
    public class MenuController : Controller
    {
        private readonly Menu_BL _menuBL;

        public MenuController(Menu_BL menuBL)
        {
            _menuBL = menuBL;
        }
        [HttpGet]
        public async Task<IActionResult> MenuEntry()
        {
            var model = new CreateMenuViewModel
            {
                ParentMenuList = await GetParentMenuListAsync()
            };
            return View("MenuEntry", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MenuEntry(CreateMenuViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ParentMenuList = await GetParentMenuListAsync();
                return View("MenuEntry", model);
            }
            try
            {
                int? parentMenuId = model.ParentMenuId.HasValue && model.ParentMenuId > 0
                    ? model.ParentMenuId
                    : null;

                var result = await _menuBL.CreateMenuAsync(
                    model.DisplayText,
                    model.ActionName,
                    model.ControllerName,
                    model.IconClass,
                    model.DisplayOrder ?? 0,
                    parentMenuId,
                    model.Status);


                int statusCode = result.StatusCode;
                string statusMessage = result.StatusMessage;

                if (statusCode == 0)
                {
                    if (statusMessage.Contains("display order", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("DisplayOrder", statusMessage);
                    }
                    else if (statusMessage.Contains("Menu Name", StringComparison.OrdinalIgnoreCase) ||
                            statusMessage.Contains("DisplayText", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("DisplayText", statusMessage);
                    }
                    else if (statusMessage.Contains("Action Name", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("ActionName", statusMessage);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, statusMessage);
                    }
                    model.ParentMenuList = await GetParentMenuListAsync();
                    return View("MenuEntry", model);
                }
                TempData["SuccessMessage"] = statusMessage;
                return RedirectToAction(nameof(MenuEntry));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error occurred: " + ex.Message);
                model.ParentMenuList = await GetParentMenuListAsync();
                return View("MenuEntry", model);
            }

        }
    
        private async Task<List<SelectListItem>> GetParentMenuListAsync()
        {
            return await _menuBL.GetParentMenuListAsync();
        }
    }
}

using CKM_ManagementSystem.Data;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MenuBL;

namespace CKM_ManagementSystem.Controllers.Menu
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Menu_BL _menuBL;

        public MenuController(ApplicationDbContext dbContext, Menu_BL menuBL)
        {
            _dbContext = dbContext;
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
            var parentMenus = await _dbContext.Menus
                .Where(m => (m.ParentMenuId == null || m.ParentMenuId == 0) && m.Deleted_Date == null)
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync();
            return parentMenus.Select(m => new SelectListItem
            {
                Value = m.MenuID.ToString(),
                Text = m.MenuName,
            }).ToList();
        }
    }
}

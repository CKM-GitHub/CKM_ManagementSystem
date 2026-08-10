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
        public async Task<IActionResult> MenuListView(string? searchTerm, int? selectedParentId, int page = 1)
        {
            int pageSize = 10;

            var rawMenuList = await _menuBL.GetMenuListAsync(searchTerm, selectedParentId, statusFilter: null);
            var menuItems = rawMenuList.Select(m => new MenuListItem
            {
                MenuID = m.MenuID,
                DisplayText = m.MenuName,
                IconClass = m.IconClass,
                ControllerName = m.ControllerName,
                ActionName = m.ActionName,
                Route = m.Route,
                MenuType = m.ParentMenuName,
                DisplayOrder = m.DisplayOrder,
                Status = m.Status
            }).ToList();
            int totalItems = menuItems.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var pagedMenuItems = menuItems
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new MenuListViewModel
            {
                SearchTerm = searchTerm,
                SelectedParentId = selectedParentId,
                Menus = pagedMenuItems,
                ParentMenuList = await GetParentMenuListAsync(),

                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                PageSize = pageSize
            };
            return View(viewModel);
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
                return RedirectToAction(nameof(MenuListView));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error occurred: " + ex.Message);
                model.ParentMenuList = await GetParentMenuListAsync();
                return View("MenuEntry", model);
            }

        }

        [HttpGet("Menu/MenuEdit/{menuId}")]
        public async Task<IActionResult> MenuEdit(int menuId)
        {
            if (menuId <= 0)
            {
                return NotFound();
            }
            var menu = await _menuBL.GetMenuByIdAsync(menuId); ;
            if (menu == null)
            {
                TempData["ErrorMessage"] = "The menu item could not be found.";
                return RedirectToAction(nameof(MenuListView));
            }
            bool isSubMenu = menu.ParentMenuId.HasValue && menu.ParentMenuId > 0;
            var model = new EditMenuViewModel
            {
                MenuID = menu.MenuID,
                DisplayText = menu.MenuName,
                ActionName = menu.ActionName,
                ControllerName = menu.ControllerName,
                IconClass = menu.IconClass,
                DisplayOrder = menu.DisplayOrder,
                ParentMenuId = menu.ParentMenuId,
                MenuType = isSubMenu ? "Sub" : "Parent",
                Status = menu.Status,
                ParentMenuList = await GetParentMenuListAsync()
            };
            return View("MenuEdit", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MenuEdit(EditMenuViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ParentMenuList = await GetParentMenuListAsync();
                return View("MenuEdit", model);
            }
            try
            {
                int? parentMenuId = model.ParentMenuId.HasValue && model.ParentMenuId > 0
                     ? model.ParentMenuId
                     : null;
                var result = await _menuBL.UpdateMenuAsync(
                    model.MenuID,
                    model.DisplayText,
                    model.ActionName,
                    model.ControllerName,
                    model.IconClass,
                    model.DisplayOrder,
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
                        ModelState.AddModelError(string.Empty, statusMessage);
                    }
                    else if (statusMessage.Contains("child", StringComparison.OrdinalIgnoreCase) ||
                            statusMessage.Contains("status", StringComparison.OrdinalIgnoreCase) ||
                            statusMessage.Contains("inactive", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("Status", statusMessage);
                    }
                    model.ParentMenuList = await GetParentMenuListAsync();
                    return View("MenuEdit", model);
                }
                TempData["SuccessMessage"] = statusMessage;
                return RedirectToAction(nameof(MenuListView));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error occurred: " + ex.Message);
                model.ParentMenuList = await GetParentMenuListAsync();
                return View("MenuEdit", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteMenu")]
        public async Task<IActionResult> DeleteMenuAsync(int menuId)
        {
            try
            {
                var result = await _menuBL.DeleteMenuAsync(menuId);

                if (result.StatusCode == 1)
                {
                    return Json(new { success = true, message = result.StatusMessage });
                }
                else
                {
                    return Json(new { success = false, message = result.StatusMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred: " + ex.Message });
            }
        }
        private async Task<List<SelectListItem>> GetParentMenuListAsync()
        {
            var parentMenus = await _dbContext.Menus
                .Where(m => (m.ParentMenuId == null || m.ParentMenuId == 0) && m.Deleted_Date == null && m.Status == true)
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

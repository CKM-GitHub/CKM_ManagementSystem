using CKM_ManagementSystem.Data;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CKM_ManagementSystem.MenuBL;

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
        public async Task<IActionResult> MenuListView(string? searchTerm, int? selectedParentId, bool? statusFilters, int page = 1)
        {
            int pageSize = 10;

            var viewModel = await _menuBL.GetPagedMenuListAsync(
                searchTerm,
                selectedParentId,
                statusFilters,
                page,
                pageSize
                );

            viewModel.ParentMenuList = await GetParentMenuListAsync();
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> MenuEntry(int? MenuID)
        {
            var model = new CreateMenuViewModel
            {
                ParentMenuList = await _menuBL.GetParentMenusForDropdownAsync()
            };
            if(MenuID.HasValue && MenuID > 0)
            {
                var menu = await _menuBL.GetMenuByIdAsync(MenuID.Value);
                if(menu == null)
                {
                    TempData["ErrorMessage"] = "The menu item could not be found.";
                    return RedirectToAction(nameof(MenuListView));
                }
                model.MenuID = menu.MenuID;
                model.DisplayText = menu.MenuName;
                model.ActionName = menu.ActionName;
                model.ControllerName = menu.ControllerName;
                model.IconClass = menu.IconClass;
                model.DisplayOrder = menu.DisplayOrder;
                model.ParentMenuId = menu.ParentMenuId;
                model.Status = menu.Status;
                model.IsSubMenu = menu.ParentMenuId.HasValue && menu.ParentMenuId > 0;
                model.MenuType = model.IsSubMenu ? "Sub" : "Parent";
            }
            return View("MenuEntry", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MenuEntry(CreateMenuViewModel model)
        {
            bool isSubMenu = string.Equals(model.MenuType, "Sub", StringComparison.OrdinalIgnoreCase);
            if (isSubMenu && (!model.ParentMenuId.HasValue || model.ParentMenuId <= 0))
            {
                ModelState.AddModelError("ParentMenuId", "Please Select Parent Menu.");
            }

            if (!ModelState.IsValid)
            {
                model.ParentMenuList = await _menuBL.GetParentMenusForDropdownAsync();
                return View("MenuEntry", model);
            }
            try
            {
                int? parentMenuId = isSubMenu && model.ParentMenuId.HasValue && model.ParentMenuId > 0
                    ? model.ParentMenuId
                    : null;

                int statusCode;
                string statusMessage;

                if (model.MenuID.HasValue && model.MenuID > 0)
                {
                    var result = await _menuBL.UpdateMenuAsync(
                        model.MenuID.Value,
                        model.DisplayText,
                        model.ActionName,
                        model.ControllerName,
                        model.IconClass,
                        model.DisplayOrder ?? 0,
                        parentMenuId,
                        isSubMenu,
                        model.Status);
                    statusCode = result.StatusCode;
                    statusMessage = result.StatusMessage;
                }
                else
                {
                    var result = await _menuBL.CreateMenuAsync(
                        model.DisplayText,
                        model.ActionName,
                        model.ControllerName,
                        model.IconClass,
                        model.DisplayOrder ?? 0,
                        parentMenuId,
                        isSubMenu,
                        model.Status);
                    statusCode = result.StatusCode;
                    statusMessage = result.StatusMessage;
                }
                if (statusCode == 0)
                {
                    if(statusMessage.Contains("Parent Menu", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("ParentMenuId", statusMessage);
                    }
                    else if (statusMessage.Contains("display order", StringComparison.OrdinalIgnoreCase))
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
                    model.ParentMenuList = await _menuBL.GetParentMenusForDropdownAsync();
                    return View("MenuEntry", model);
                }
                if (statusCode == 1)
                {
                    TempData["SuccessMessage"] = statusMessage;
                    model.ParentMenuList = await _menuBL.GetParentMenusForDropdownAsync();
                    return View("MenuEntry", model);
                }
                ModelState.AddModelError(string.Empty, statusMessage ?? "Unexpected status returned");
                model.ParentMenuList = await _menuBL.GetParentMenusForDropdownAsync();
                return View("MenuEntry", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error occurred: " + ex.Message);
                model.ParentMenuList = await _menuBL.GetParentMenusForDropdownAsync();
                return View("MenuEntry", model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteMenu")]
        public async Task<IActionResult> DeleteMenuAsync(int menuId, int page=1)
        {
            try
            {
                var result = await _menuBL.DeleteMenuAsync(menuId);

                if (result.StatusCode == 1)
                {
                    TempData["SuccessMessage"] = result.StatusMessage;
                }
                else
                {
                    TempData["ErrorMessage"] = result.StatusMessage;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error occurred: " + ex.Message;
            }
            return RedirectToAction(nameof(MenuListView), new {page = page});
        }
        private async Task<List<SelectListItem>> GetParentMenuListAsync()
        {
            return await _menuBL.GetParentMenuListAsync();
        }
    }
}

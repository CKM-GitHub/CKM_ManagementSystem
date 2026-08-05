using CKM_ManagementSystem.Data;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MenuBL;

namespace CKM_ManagementSystem.Controllers
{
    public class MenuController : Controller
    {
        private readonly  ApplicationDbContext _dbContext;
        private readonly Menu_BL _menuBL;
        
        public MenuController(ApplicationDbContext dbContext, Menu_BL menuBL)
        {
            _dbContext = dbContext;
            _menuBL = menuBL;
        }

        [HttpGet]
        public async Task<IActionResult> MenuListView(string? searchTerm, int? selectedParentId, int page=1)
        {
            int pageSize = 10;
           
            var rawMenuList = await _menuBL.GetMenuListAsync(searchTerm, selectedParentId);
            var menuItems = rawMenuList.Select(m => new CKM_ManagementSystem.Models.ViewModels.MenuListItem
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
                int? parentMenuId = (model.ParentMenuId.HasValue && model.ParentMenuId > 0)
                    ? model.ParentMenuId
                    : null;

                var result = await _menuBL.CreateMenuAsync(
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
                    else if(statusMessage.Contains("Menu Name", StringComparison.OrdinalIgnoreCase) ||
                            statusMessage.Contains("DisplayText", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("DisplayText", statusMessage);
                    }
                    else if(statusMessage.Contains("Action Name", StringComparison.OrdinalIgnoreCase))
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
        
        private async Task<List<SelectListItem>> GetParentMenuListAsync()
        {
            var parentMenus = await _dbContext.Menus
                .Where(m => (m.ParentMenuId == null || m.ParentMenuId == 0)  && m.Deleted_Date == null)
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

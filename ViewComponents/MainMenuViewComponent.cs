using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels.MainMenu;
using Microsoft.AspNetCore.Mvc;

namespace CKM_ManagementSystem.ViewComponents
{
    public class MainMenuViewComponent : ViewComponent
    {
        private readonly MainMenuBL _mainMenuBL;

        public MainMenuViewComponent(MainMenuBL mainMenuBL)
        {
            _mainMenuBL = mainMenuBL;
        }

        public IViewComponentResult Invoke() {

             string? staffCode = HttpContext.Session.GetString("StaffCode");

            if (string.IsNullOrWhiteSpace(staffCode))
            {
                return View(new List<MainMenuViewModel>());
            }

            var menuList = _mainMenuBL.GetMainMenus(staffCode);
            return View(menuList);
        }
    }
}

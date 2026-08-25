using CKM_ManagementSystem.BL;
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
                staffCode = "CKM-0001";
            }

            var menuList = _mainMenuBL.GetMainMenus(staffCode);
            return View(menuList);
        }
    }
}

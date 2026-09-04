using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels.MainMenu;
using Microsoft.AspNetCore.Mvc;

namespace CKM_ManagementSystem.Controllers.MainMenu
{
    public class MainMenuController : Controller
    {
        private readonly MainMenuBL _mainMenuBL;

        public MainMenuController(MainMenuBL mainMenuBL)
        {
            _mainMenuBL = mainMenuBL;
        }

        [HttpGet]
        public IActionResult Index()
        {
              string? staffCode = User.FindFirst("StaffCode")?.Value;

            if (string.IsNullOrWhiteSpace(staffCode))
            {
                return PartialView("MainMenu", new List<MainMenuViewModel>());
            }


            List<MainMenuViewModel>menuList= _mainMenuBL.GetMainMenus(staffCode);

            return PartialView("MainMenu",menuList);
        }

        [HttpGet]
        public IActionResult Profile()
        {
            string? staffCode = User.FindFirst("StaffCode")?.Value;

            if (string.IsNullOrWhiteSpace(staffCode))
            {
                return PartialView("Profile", (MainMenuViewModel?)null);
            }


            List<MainMenuViewModel>menuList=_mainMenuBL.GetMainMenus(staffCode);

            MainMenuViewModel? profile = menuList.FirstOrDefault();

            return PartialView("Profile",profile);
        }
    }
}
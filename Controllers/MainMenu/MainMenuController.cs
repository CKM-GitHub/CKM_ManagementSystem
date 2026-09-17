using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels.MainMenu;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index()
        {
            string? staffCode = User.FindFirst("StaffCode")?.Value;

            if (string.IsNullOrWhiteSpace(staffCode))
            {
                return PartialView("MainMenu", new List<MainMenuViewModel>());
            }


            List<MainMenuViewModel> menuList =
                            await _mainMenuBL.GetMainMenus(staffCode);
            return PartialView("MainMenu", menuList);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            string? staffCode = User.FindFirst("StaffCode")?.Value;

            if (string.IsNullOrWhiteSpace(staffCode))
            {
                return PartialView("Profile", (MainMenuViewModel?)null);
            }


            List<MainMenuViewModel> menuList = await _mainMenuBL.GetMainMenus(staffCode);

            MainMenuViewModel? profile = menuList.FirstOrDefault();

            return PartialView("Profile", profile);
        }

        [HttpGet]
        public async Task<IActionResult> SubNavigation(
            string currentController,
            string currentAction)
        {
            string? staffCode = User.FindFirst("StaffCode")?.Value;

            if (string.IsNullOrWhiteSpace(staffCode))
            {
                return PartialView(
                    "SubNavigation",
                    new List<MainMenuViewModel>()
                   );
            }
            List<MainMenuViewModel> menuList =
                await _mainMenuBL.GetMainMenus(staffCode);

            MainMenuViewModel? currentParent = null;

            foreach (var parent in menuList)
            {
                var currentSubMenu = parent.SubMenus
                    .FirstOrDefault(menu =>
                    string.Equals(
                        menu.ControllerName,
                        currentController,
                        StringComparison.OrdinalIgnoreCase
                        )
                        &&
                     string.Equals(
                         menu.ActionName,
                         currentAction,
                         StringComparison.OrdinalIgnoreCase)
                    );

                if (currentSubMenu != null)
                {
                    currentParent = parent;
                    break;
                }
            }
            if (currentParent == null)
            {
                return PartialView(
                    "SubNavigation",
                    new List<MainMenuViewModel>()
                    );
            }

            ViewBag.CurrentController = currentController;
            ViewBag.CurrentAction = currentAction;

            return PartialView (
                "SubNavigation",
                currentParent.SubMenus);
        }
    }
}
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

            List<MainMenuViewModel> navigationPath =
                new List<MainMenuViewModel>();

            bool found = TryBuildMenuPath(
                 menuList,
                 currentController,
                 currentAction,
                 navigationPath
                );
            if (!found)
            {
                navigationPath.Clear();
            }
            return PartialView(
                "SubNavigation",
                navigationPath);



        }
        private bool TryBuildMenuPath(
     IEnumerable<MainMenuViewModel> menus,
     string currentController,
     string currentAction,
     List<MainMenuViewModel> path)
        {
            foreach (var menu in menus)
            {
                path.Add(menu);

                // Child menus ကို အရင်ရှာမယ်
                if (
                    menu.SubMenus != null &&
                    menu.SubMenus.Count > 0
                )
                {
                    bool foundInChildren =
                        TryBuildMenuPath(
                            menu.SubMenus,
                            currentController,
                            currentAction,
                            path
                        );

                    if (foundInChildren)
                    {
                        return true;
                    }
                }

                // Children ထဲမှာမတွေ့မှ current menu ကိုစစ်မယ်
                bool isCurrentPage =
                    string.Equals(
                        menu.ControllerName,
                        currentController,
                        StringComparison.OrdinalIgnoreCase
                    )
                    &&
                    string.Equals(
                        menu.ActionName,
                        currentAction,
                        StringComparison.OrdinalIgnoreCase
                    );

                if (isCurrentPage)
                {
                    return true;
                }

                path.RemoveAt(
                    path.Count - 1
                );
            }

            return false;
        }
    }
}
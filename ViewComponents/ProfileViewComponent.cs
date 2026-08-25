using CKM_ManagementSystem.BL;
using Microsoft.AspNetCore.Mvc;

namespace CKM_ManagementSystem.ViewComponents
{
    public class ProfileViewComponent : ViewComponent
    {
        private readonly MainMenuBL _mainMenuBl;

        public ProfileViewComponent(MainMenuBL mainMenuBl)
        {
            _mainMenuBl = mainMenuBl;
        }

        public IViewComponentResult Invoke()
        {
            string? staffCode = HttpContext.Session.GetString("StaffCode");


            //testing
            if (string.IsNullOrWhiteSpace(staffCode)) 
            {
                staffCode = "CKM-0001";
            
            }
            var menulist=_mainMenuBl.GetMainMenus(staffCode);

            var profile=menulist.FirstOrDefault();

            return View(profile);
        }
    }
}

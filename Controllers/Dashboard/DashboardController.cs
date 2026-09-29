using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CKM_ManagementSystem.Controllers.LoginUser.LoginUsersController;

namespace CKM_ManagementSystem.Controllers.Dashboard
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DashboardBL _dashboardBL;
        private readonly MainMenuBL _mainMenuBL;  

        public DashboardController(DashboardBL dashboardBL, MainMenuBL mainMenuBL)
        {
            _dashboardBL = dashboardBL;
            _mainMenuBL = mainMenuBL;
        }

        public async Task<IActionResult> Dashboard()
        {
            var staffCode = User.FindFirst(CustomClaimTypes.staffCode)?.Value;

            if (string.IsNullOrEmpty(staffCode))
                return RedirectToAction("Login", "LoginUsers");

            var menus = await _mainMenuBL.GetMainMenus(staffCode);
            var profile = menus.FirstOrDefault();  

            var tasks = await _dashboardBL.GetTaskStatusSummaryAsync(staffCode);
            var announcements = await _dashboardBL.GetAnnouncementListAsync(staffCode);
            var projects = await _dashboardBL.GetProjectTaskSummaryAsync(staffCode);

            var model = new DashboardViewModel
            {
                StaffCode = profile?.StaffCode ?? staffCode,
                UserName = profile?.UserName ?? "User",
                ImageURL = profile?.ImageURL,

                TaskStatusSummary = tasks,
                Announcements = announcements,
                ProjectTaskSummary = projects
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AnnouncementDetails(int id)
        {
            var model = await _dashboardBL.GetAnnouncementDetailsAsync(id);

            if (model == null)
                return NotFound();

            return Json(model); 
        }
    }
}
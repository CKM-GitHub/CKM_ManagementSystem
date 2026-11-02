using CKM_ManagementSystem;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels.Announcements;
using Microsoft.AspNetCore.Mvc;




namespace CKM_ManagementSystem.Controllers.Announcement
{
    public class AnnouncementsController : Controller
    {
        private readonly AnnouncementsBL announcementsBL;

        public AnnouncementsController(
            AnnouncementsBL announcementsBL)
        {
            this.announcementsBL = announcementsBL;
        }

        [HttpGet]
        public async Task<IActionResult> AnnouncementList(
             AnnouncementListFilterViewModel filter,
             int pageNumber = 1
 )
        {
            var model =
                await this.announcementsBL
                    .GetAnnouncementListAsync(
                        filter,
                        pageNumber,
                        6
                    );

            return View(
                "/Views/Announcements/AnnouncementList.cshtml",
                model
            );
        }
        [HttpGet]
        public async Task<IActionResult> GetAnnouncementDetail(int id)
        {
            var announcement =
                await  this.announcementsBL
                    .GetAnnouncementDetailAsync(id);

            if (announcement == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Announcement not found."
                });
            }

            return Json(new
            {
                success = true,
                data = announcement
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            bool success =
                await this.announcementsBL
                    .DeleteAnnouncementAsync(id);

            if (success)
            {
                return Json(new
                {
                    success = true,
                    message = "Announcement deleted successfully."
                });
            }

            return Json(new
            {
                success = false,
                message = "Failed to delete the announcement."
            });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.Models.ViewModels.Projects;
using CKM_ManagementSystem.Services;

namespace CKM_ManagementSystem.Controllers.ProjectEntry
{
    public class ProjectController : Controller
    {
        private readonly ProjectService _projectService;

        public ProjectController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        [ActionName("ProjectEntry")]
        public IActionResult ProjectEntry()
        {
            ViewBag.Managers = _projectService.GetActiveManagers();
            return View("~/Views/Project/ProjectEntry.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data submitted. Please check the fields." });
            }

            try
            {
                var (isSuccess, message) = await _projectService.SaveProjectAsync(model);

                if (isSuccess)
                {
                    return Json(new { success = true, message = message });
                }

              
                return Json(new { success = false, message = message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Controller Error: " + ex.Message });
            }
        }
    }
}
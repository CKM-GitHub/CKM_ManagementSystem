using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.Models.ViewModels.Projects;
using CKM_ManagementSystem.BL;

namespace CKM_ManagementSystem.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ProjectBL _projectBL;

        
        public ProjectController(ProjectBL projectBL)
        {
            _projectBL = projectBL;
        }

        [HttpGet]
        public IActionResult ProjectEntry(string id)
        {
            var model = new ProjectEntryViewModel();

            if (!string.IsNullOrEmpty(id))
            {
                model = _projectBL.GetProjectById(id);
            }

            ViewBag.Managers = _projectBL.GetActiveManagers();
            return View("~/Views/Project/ProjectEntry.cshtml", model);
        }

        [HttpPost]
        public IActionResult CheckDuplicateCode(string projectCode)
        {
            bool isDuplicate = _projectBL.IsDuplicateProjectCode(projectCode);
            return Json(new { isDuplicate });
        }

        [HttpPost]
        public IActionResult SaveProject(ProjectEntryViewModel model, bool isEdit)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data submitted. Please check mandatory fields." });
            }

            try
            {
                string errorMessage;
                bool isSuccess = _projectBL.SaveProject(model, isEdit, out errorMessage);

                if (isSuccess)
                {
                    string msg = isEdit ? "Project updated successfully." : "Project registered successfully.";
                    return Json(new { success = true, isEdit = isEdit, message = msg });
                }

                return Json(new { success = false, message = errorMessage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Controller Error: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ProjectList()
        {
            return View("~/Views/Project/ProjectList.cshtml");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels.Projects;

namespace CKM_ManagementSystem.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ProjectBL _projectBL;

       
        public ProjectController(ProjectBL projectBL)
        {
            _projectBL = projectBL;
        }

        private void BindDropdowns()
        {
            ViewBag.Managers = _projectBL.GetActiveManagers();
            ViewBag.Departments = _projectBL.GetDepartments();
        }

        [HttpGet]
        public IActionResult ProjectEntry()
        {
            BindDropdowns();
            var model = new ProjectEntryViewModel
            {
                IsEdit = false
            };
            return View("ProjectEntry", model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            BindDropdowns();
            var model = new ProjectEntryViewModel
            {
                IsEdit = false
            };
            return View("ProjectEntry", model);
        }

        [HttpGet]
        public IActionResult Edit(string projectCode)
        {
            if (string.IsNullOrEmpty(projectCode))
            {
                return NotFound();
            }

            BindDropdowns();
            var model = _projectBL.GetProjectById(projectCode);
            if (model == null || string.IsNullOrEmpty(model.ProjectCode))
            {
                return NotFound();
            }

            model.IsEdit = true;
            return View("ProjectEntry", model);
        }

        [HttpPost]
        public IActionResult SearchProjectMembers(string searchText, string departmentCode)
        {
            var result = _projectBL.SearchProjectMembers(searchText, departmentCode);
            return Json(result);
        }

        [HttpPost]
        public IActionResult CheckDuplicateProjectCode(string projectCode)
        {
            bool isDuplicate = _projectBL.IsDuplicateProjectCode(projectCode);
            return Json(new { isDuplicate = isDuplicate });
        }

        [HttpPost]
        public IActionResult CheckDuplicateProjectName(string projectName, string projectCode, bool isEdit)
        {
            string? codeToExclude = isEdit ? projectCode : null;
            bool isDuplicate = _projectBL.IsDuplicateProjectName(projectName, codeToExclude);
            return Json(new { isDuplicate = isDuplicate });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveProject([FromBody] ProjectEntryViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Invalid project data." });
            }

            if (!model.IsEdit && string.IsNullOrWhiteSpace(model.ProjectCode))
            {
                return Json(new { success = false, message = "Project Code is required." });
            }

            if (model.EndDate < model.StartDate)
            {
                return Json(new { success = false, message = "Target End Date cannot be earlier than Start Date." });
            }

            string errorMessage;
            bool isSuccess = _projectBL.SaveProject(model, model.IsEdit, out errorMessage);

            if (isSuccess)
            {
                string msg = model.IsEdit ? "Project updated successfully!" : "Project created successfully!";
                return Json(new { success = true, message = msg });
            }

            return Json(new { success = false, message = string.IsNullOrEmpty(errorMessage) ? "An error occurred while saving the project." : errorMessage });
        }
    }
}
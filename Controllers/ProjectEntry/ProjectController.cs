using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using CKM_ManagementSystem.Authorization;
using CKM_ManagementSystem.Permissions;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels.Project;
using System.Linq;

namespace CKM_ManagementSystem.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ProjectBL _projectBL;
        private readonly CurrentUserPermission _permission;

        public ProjectController(ProjectBL projectBL, CurrentUserPermission permission)
        {
            _projectBL = projectBL;
            _permission = permission;
        }

        private void BindDropdowns()
        {
            var rawManagers = _projectBL.GetActiveManagers();

            ViewBag.Managers = rawManagers.Select(m => new SelectListItem
            {
                Value = m.Value,
                Text = !string.IsNullOrEmpty(m.Text) && m.Text.Length > 30
                    ? m.Text.Substring(0, 27) + "..."
                    : m.Text
            }).ToList();

            var rawDepartments = _projectBL.GetDepartments();

            ViewBag.Departments = rawDepartments.Select(d => new SelectListItem
            {
                Value = d.Value,
                Text = !string.IsNullOrEmpty(d.Text) && d.Text.Length > 30
                    ? d.Text.Substring(0, 27) + "..."
                    : d.Text
            }).ToList();
        }

        [HttpGet]
        [Authorize(Policy = "Permission.Project.Read")]
        public IActionResult ProjectList(string? searchKeyword, string? status, int pageNumber = 1)
        {
            const int pageSize = 6;

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            ProjectListPagedViewModel model = _projectBL.GetProjectList(
                searchKeyword,
                status,
                pageNumber,
                pageSize);

            if (model.TotalPages > 0 && pageNumber > model.TotalPages)
            {
                model = _projectBL.GetProjectList(
                    searchKeyword,
                    status,
                    model.TotalPages,
                    pageSize);
            }

            model.CanWrite = _permission.CanWrite(MenuIDs.Project);
            model.CanDelete = _permission.CanDelete(MenuIDs.Project);

            return View("ProjectList", model);
        }

        [HttpGet]
        [Authorize(Policy = "Permission.Project.Read")]
        public IActionResult ProjectEntry()
        {
            BindDropdowns();

            ProjectEntryViewModel model = new ProjectEntryViewModel
            {
                IsEdit = false,
                CanWrite = _permission.CanWrite(MenuIDs.Project)
            };

            return View("ProjectEntry", model);
        }

        [HttpGet]
        [Authorize(Policy = "Permission.Project.Read")]
        public IActionResult Create()
        {
            BindDropdowns();

            ProjectEntryViewModel model = new ProjectEntryViewModel
            {
                IsEdit = false,
                CanWrite = _permission.CanWrite(MenuIDs.Project)
            };

            return View("ProjectEntry", model);
        }

        [HttpGet]
        [Authorize(Policy = "Permission.Project.Read")]
        public IActionResult Edit(string projectCode)
        {
            if (string.IsNullOrEmpty(projectCode))
            {
                return NotFound();
            }

            BindDropdowns();

            ProjectEntryViewModel model = _projectBL.GetProjectById(projectCode);

            if (model == null || string.IsNullOrEmpty(model.ProjectCode))
            {
                return NotFound();
            }

            model.IsEdit = true;
            model.CanWrite = _permission.CanWrite(MenuIDs.Project);

            return View("ProjectEntry", model);
        }

        [HttpPost]
        [Authorize(Policy = "Permission.Project.Read")]
        public IActionResult SearchProjectMembers(string searchText, string departmentCode)
        {
            var result = _projectBL.SearchProjectMembers(searchText, departmentCode);
            return Json(result);
        }

        [HttpPost]
        [Authorize(Policy = "Permission.Project.Read")]
        public IActionResult CheckDuplicateProjectCode(string projectCode)
        {
            bool isDuplicate = _projectBL.IsDuplicateProjectCode(projectCode);
            return Json(new { isDuplicate });
        }

        [HttpPost]
        [Authorize(Policy = "Permission.Project.Read")]
        public IActionResult CheckDuplicateProjectName(string projectName, string projectCode, bool isEdit)
        {
            string? codeToExclude = isEdit ? projectCode : null;
            bool isDuplicate = _projectBL.IsDuplicateProjectName(projectName, codeToExclude);
            return Json(new { isDuplicate });
        }

        [HttpPost]
        [Authorize(Policy = "Permission.Project.Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProject(string projectCode)
        {
            if (string.IsNullOrWhiteSpace(projectCode))
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid Project Code."
                });
            }

            var result = _projectBL.DeleteProject(projectCode);

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        [HttpPost]
        [Authorize(Policy = "Permission.Project.Write")]
        [ValidateAntiForgeryToken]
        public IActionResult SaveProject([FromBody] ProjectEntryViewModel model)
        {
            if (model == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid project data."
                });
            }

            if (!model.IsEdit && string.IsNullOrWhiteSpace(model.ProjectCode))
            {
                return Json(new
                {
                    success = false,
                    message = "Project Code is required."
                });
            }

            if (model.EndDate < model.StartDate)
            {
                return Json(new
                {
                    success = false,
                    message = "Target End Date cannot be earlier than Start Date."
                });
            }

            string errorMessage;
            bool isSuccess = _projectBL.SaveProject(model, model.IsEdit, out errorMessage);

            if (isSuccess)
            {
                string msg = model.IsEdit
                    ? "Project updated successfully!"
                    : "Project created successfully!";

                return Json(new
                {
                    success = true,
                    message = msg
                });
            }

            return Json(new
            {
                success = false,
                message = string.IsNullOrEmpty(errorMessage)
                    ? "An error occurred while saving the project."
                    : errorMessage
            });
        }
    }
}
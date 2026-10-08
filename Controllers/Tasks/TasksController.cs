using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.Models.ViewModels.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CKM_ManagementSystem.Controllers
{
    public class TasksController : Controller
    {
        private readonly TasksBL _taskBL;

        public TasksController(TasksBL tasksBL)
        {
            _taskBL = tasksBL;
        }

        [HttpGet]
        [Route("Tasks/TaskLists")]
        public async Task<IActionResult> TaskLists([FromQuery] TaskManagementFilterViewModel filter)
        {
            filter ??= new TaskManagementFilterViewModel();
            filter.CurrentPage = filter.CurrentPage < 1 ? 1 : filter.CurrentPage;
            filter.PageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            await PopulateDropdownsAsync(filter);

            filter.TaskListData = await _taskBL.GetTaskManagementListAsync(filter);

            filter.TotalItems = filter.TaskListData.Count; 

            return View("TaskLists", filter);
        }

        [HttpPost]
        [Route("Tasks/TaskLists")]
        [ActionName("TaskListsPost")]
        public async Task<IActionResult> TaskListsPost([FromForm] TaskManagementFilterViewModel filter)
        {
            filter.CurrentPage = filter.CurrentPage < 1 ? 1 : filter.CurrentPage;
            filter.PageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            filter.TaskListData = await _taskBL.GetTaskManagementListAsync(filter);
            filter.TotalItems = filter.TaskListData.Count;

            await PopulateDropdownsAsync(filter);

            return View("TaskLists", filter);
        }
        private async Task PopulateDropdownsAsync(TaskManagementFilterViewModel filter)
        {
            var projects = await _taskBL.GetProjectsAsync();
            var persons = await _taskBL.GetPersonsInChargeAsync();
            var assignees = await _taskBL.GetManagementAssigneesAsync();
            var priorities = await _taskBL.GetPrioritiesAsync();
            var statuses = await _taskBL.GetStatusesAsync();

            filter.ProjectList = projects
                .Select(x => new SelectListItem { Value = x.Value, Text = x.Text })
                .ToList();

            filter.PersonInChargeList = persons
                .Select(x => new SelectListItem { Value = x.Value, Text = x.Text })
                .ToList();

            filter.AssigneeList = assignees
                .Select(x => new SelectListItem { Value = x.Value, Text = x.Text })
                .ToList();

            filter.PriorityList = priorities
                .Select(x => new SelectListItem { Value = x.Value, Text = x.Text })
                .ToList();

            filter.StatusList = statuses
                .Select(x => new SelectListItem { Value = x.Value, Text = x.Text })
                .ToList();
        }

        [HttpGet]
        public async Task<IActionResult> GetAssignees(string projectCode)
        {
            var assignees = await _taskBL.GetAssigneesAsync(projectCode);
            return Json(assignees);
        }


        [HttpGet]
        public async Task<IActionResult> MyTasksOverview(
            MyTasksOverviewViewModel model)
        {
            string? loginStaffCode =
                User.FindFirst("StaffCode")?.Value;

            if (string.IsNullOrWhiteSpace(loginStaffCode))
            {
                return Unauthorized();
            }

            model.ProjectList =
                await _taskBL.GetProjectsAsync();

            model.PriorityList =
                await _taskBL.GetPrioritiesAsync();

            model.StatusList =
                await _taskBL.GetStatusesAsync();

            model.TaskList =
                await _taskBL.GetMyTaskListAsync(
                    model.Filter,
                    loginStaffCode
                );

            model.StatusSummary =
                await _taskBL.GetMyTaskStatusSummaryAsync(
                    loginStaffCode
                );

            return View(
                "~/Views/Tasks/MyTasksOverview.cshtml",
                model
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMyTask(
            MyTaskUpdateViewModel model,
            string? returnUrl)
        {
            string? loginStaffCode =
                User.FindFirst("StaffCode")?.Value;

            if (string.IsNullOrWhiteSpace(loginStaffCode))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                TempData["MyTaskError"] =
                    "Invalid task data.";

                if (
                    !string.IsNullOrWhiteSpace(returnUrl) &&
                    Url.IsLocalUrl(returnUrl)
                )
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction(
                    nameof(MyTasksOverview)
                );
            }

            if (
                model.StartDate.HasValue &&
                model.EndDate.HasValue &&
                model.EndDate.Value <
                model.StartDate.Value
            )
            {
                TempData["MyTaskError"] =
                    "End Date cannot be earlier than Start Date.";

                if (
                    !string.IsNullOrWhiteSpace(returnUrl) &&
                    Url.IsLocalUrl(returnUrl)
                )
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction(
                    nameof(MyTasksOverview)
                );
            }

            bool updated =
                await _taskBL.UpdateMyTaskAsync(
                    model,
                    loginStaffCode
                );

            if (updated)
            {
                TempData["MyTaskMessage"] =
                    "Task updated successfully.";
            }
            else
            {
                TempData["MyTaskError"] =
                    "Task update failed.";
            }

            if (
                !string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl)
            )
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction(
                nameof(MyTasksOverview)
            );
        }
    }
}
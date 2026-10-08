using CKM_ManagementSystem.BL;
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


        // =====================================================
        // Task Management
        // =====================================================

        [HttpGet]
        [Route("Tasks/TaskLists")]
        public async Task<IActionResult> TaskLists(
            [FromQuery] TaskManagementFilterViewModel filter)
        {
            filter ??= new TaskManagementFilterViewModel();

            filter.CurrentPage =
                filter.CurrentPage < 1
                    ? 1
                    : filter.CurrentPage;

            filter.PageSize =
                filter.PageSize <= 0
                    ? 5
                    : filter.PageSize;

            await PopulateDropdownsAsync(filter);

            await LoadPagedTaskListAsync(filter);

            return View("TaskLists", filter);
        }


        [HttpPost]
        [Route("Tasks/TaskLists")]
        [ActionName("TaskListsPost")]
        public async Task<IActionResult> TaskListsPost(
            [FromForm] TaskManagementFilterViewModel filter)
        {
            filter ??= new TaskManagementFilterViewModel();

            filter.CurrentPage = 1;

            filter.PageSize =
                filter.PageSize <= 0
                    ? 5
                    : filter.PageSize;

            await PopulateDropdownsAsync(filter);

            await LoadPagedTaskListAsync(filter);

            return View("TaskLists", filter);
        }


        private async Task LoadPagedTaskListAsync(
            TaskManagementFilterViewModel filter)
        {
            var allTasks =
                await _taskBL.GetTaskManagementListAsync(filter);

            filter.TotalItems =
                allTasks.Count;

            if (filter.TotalPages > 0 &&
                filter.CurrentPage > filter.TotalPages)
            {
                filter.CurrentPage =
                    filter.TotalPages;
            }

            if (filter.CurrentPage < 1)
            {
                filter.CurrentPage = 1;
            }

            int skip =
                (filter.CurrentPage - 1)
                * filter.PageSize;

            filter.TaskListData =
                allTasks
                    .Skip(skip)
                    .Take(filter.PageSize)
                    .ToList();
        }


        private async Task PopulateDropdownsAsync(
            TaskManagementFilterViewModel filter)
        {
            var projects =
                await _taskBL.GetProjectsAsync();

            var persons =
                await _taskBL.GetPersonsInChargeAsync();

            var assignees =
                await _taskBL.GetManagementAssigneesAsync();

            var priorities =
                await _taskBL.GetPrioritiesAsync();

            var statuses =
                await _taskBL.GetStatusesAsync();

            filter.ProjectList =
                projects.Select(x =>
                    new SelectListItem
                    {
                        Value = x.Value,
                        Text = x.Text
                    }).ToList();

            filter.PersonInChargeList =
                persons.Select(x =>
                    new SelectListItem
                    {
                        Value = x.Value,
                        Text = x.Text
                    }).ToList();

            filter.AssigneeList =
                assignees.Select(x =>
                    new SelectListItem
                    {
                        Value = x.Value,
                        Text = x.Text
                    }).ToList();

            filter.PriorityList =
                priorities.Select(x =>
                    new SelectListItem
                    {
                        Value = x.Value,
                        Text = x.Text
                    }).ToList();

            filter.StatusList =
                statuses.Select(x =>
                    new SelectListItem
                    {
                        Value = x.Value,
                        Text = x.Text
                    }).ToList();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTask(
            TaskManagementUpdateViewModel model,
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
                TempData["ErrorMessage"] =
                    "Invalid task data.";

                if (!string.IsNullOrWhiteSpace(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction(
                    nameof(TaskLists));
            }

            bool updated =
                await _taskBL.UpdateTaskAsync(
                    model,
                    loginStaffCode);

            if (updated)
            {
                TempData["SuccessMessage"] =
                    "Task updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    "Task update failed.";
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction(
                nameof(TaskLists));
        }


        [HttpGet]
        public async Task<IActionResult> GetAssignees(
            string projectCode)
        {
            var assignees =
                await _taskBL.GetAssigneesAsync(
                    projectCode);

            return Json(assignees);
        }


        // =====================================================
        // My Tasks Overview
        // =====================================================

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

            model.CurrentPage =
                model.CurrentPage < 1
                    ? 1
                    : model.CurrentPage;

            model.PageSize =
                model.PageSize <= 0
                    ? 5
                    : model.PageSize;


            // Dropdowns
            model.ProjectList =
                await _taskBL.GetProjectsAsync();

            model.PriorityList =
                await _taskBL.GetPrioritiesAsync();

            model.StatusList =
                await _taskBL.GetStatusesAsync();


            // Get all filtered tasks assigned to login user
            var allTasks =
                await _taskBL.GetMyTaskListAsync(
                    model.Filter,
                    loginStaffCode);


            // Pagination
            model.TotalItems =
                allTasks.Count;

            if (model.TotalPages > 0 &&
                model.CurrentPage > model.TotalPages)
            {
                model.CurrentPage =
                    model.TotalPages;
            }

            if (model.CurrentPage < 1)
            {
                model.CurrentPage = 1;
            }

            int skip =
                (model.CurrentPage - 1)
                * model.PageSize;

            model.TaskList =
                allTasks
                    .Skip(skip)
                    .Take(model.PageSize)
                    .ToList();


            // Summary cards remain overall
            // assigned-task summary for login user
            model.StatusSummary =
                await _taskBL.GetMyTaskStatusSummaryAsync(
                    loginStaffCode);


            return View(
                "~/Views/Tasks/MyTasksOverview.cshtml",
                model);
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

                if (!string.IsNullOrWhiteSpace(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction(
                    nameof(MyTasksOverview));
            }

            if (model.StartDate.HasValue &&
                model.EndDate.HasValue &&
                model.EndDate.Value <
                model.StartDate.Value)
            {
                TempData["MyTaskError"] =
                    "End Date cannot be earlier than Start Date.";

                if (!string.IsNullOrWhiteSpace(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction(
                    nameof(MyTasksOverview));
            }

            bool updated =
                await _taskBL.UpdateMyTaskAsync(
                    model,
                    loginStaffCode);

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

            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction(
                nameof(MyTasksOverview));
        }
    }
}
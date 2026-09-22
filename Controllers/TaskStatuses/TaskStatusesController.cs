using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels.TaskStatuses;
using Microsoft.AspNetCore.Mvc;

namespace CKM_ManagementSystem.Controllers.TaskStatuses
{
    public class TaskStatusesController : Controller
    {
        private readonly TaskStatusesBL _taskStatusesBL;

        public TaskStatusesController(TaskStatusesBL taskStatusesBL)
        {
            _taskStatusesBL = taskStatusesBL;
        }

        [HttpGet]
        public async Task<IActionResult> TaskStatusesListView(string? Search = null, int PageNumber = 1, int PageSize = 10)
        {
            var model = await _taskStatusesBL.GetTaskStatusesListAsync(Search, PageNumber, PageSize);
            return View(model);
        }

        [HttpGet]
        public IActionResult TaskStatusesEntry()
        {
            var model = new CreateTaskStatusesViewModel();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetTaskStatus(string statusCode)
        {
            if (string.IsNullOrWhiteSpace(statusCode))
            {
                return Json(new { success = false, message = "Status Code is required. " });
            }
            var status = await _taskStatusesBL.GetTaskStatusByCodeAsync(statusCode);
            if (status == null)
            {
                return Json(new { success = false, message = "Data not found." });
            }
            status.Mode = "Edit";
            return Json(new { success = true, data = status });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaskStatusesEntry(CreateTaskStatusesViewModel model)
        {
            if (string.Equals(model.Mode, "Edit", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.Remove("Status_Code");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return Json(new { success = false, message = string.Join("<br/>", errors) });
            }
            try
            {
                TaskStatusesBL.TaskStatusesResult result;
                if (string.Equals(model.Mode, "Edit", StringComparison.OrdinalIgnoreCase))
                {
                    result = await _taskStatusesBL.UpdateTaskStatusesAsync(
                        model.Status_Code,
                        model.Status_Name,
                        model.Description,
                        model.SortOrder
                    );
                }
                else
                {
                    result = await _taskStatusesBL.CreateTaskStatusesAsync(
                        model.Status_Code,
                        model.Status_Name,
                        model.Description,
                        model.SortOrder);
                }
                if (result.ResponseCode == 1)
                {
                    return Json(new { success = true, message = result.ResponseMessage });
                }
                else
                {
                    return Json(new { success = false, message = result.ResponseMessage });
                }
            }
            catch (Exception ex)
            {
                string errorMessage;
                if (ex.Message.Contains("String or binary data would be truncated") ||
                    (ex.InnerException != null && ex.InnerException.Message.Contains("String or binary data would be truncated")))
                {
                    errorMessage = "The input text exceeds the maximum character limit allowed.";
                }
                else
                {
                    errorMessage = "A system error has occurred. Please try again later.";
                }
                return Json(new { success = false, message = errorMessage });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTaskStatuses(string statusCode)
        {
            try
            {
                var result = await _taskStatusesBL.DeleteTaskStatusesAsync(statusCode);
                if (result.ResponseCode == 1)
                {
                    return Json(new { success = true, message = result.ResponseMessage});
                }
                else
                {
                    return Json(new { success = false, message = result.ResponseMessage });
                }
            }
            catch (Exception ex)
            {
                string errorMessage;
                if (ex.Message.Contains("REFERENCE constraint") ||
                     (ex.InnerException != null && ex.InnerException.Message.Contains("REFERENCE constraint")))
                {
                    errorMessage = "Cannot delete this status because it is currently in use.";
                }
                else
                {
                    errorMessage = "An error occurred: " + ex.Message;
                }
                return Json(new {success = false, message = errorMessage});
            }
        }
    }
}
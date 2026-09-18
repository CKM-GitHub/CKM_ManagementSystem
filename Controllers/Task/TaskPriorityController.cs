using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels.Task;
using CKM_ManagementSystem.Models.ViewModels.TaskPriorities;
using Microsoft.AspNetCore.Mvc;

namespace CKM_ManagementSystem.Controllers.Task
{
    public class TaskPriorityController : Controller
    {
        private readonly TaskPriorityBL _taskPriorityBL;

        public TaskPriorityController(TaskPriorityBL taskPriorityBL)
        {
            _taskPriorityBL = taskPriorityBL;
        }

        public async Task<IActionResult> TaskPriorityList(
            string? Search,
            int PageNumber = 1,
            int PageSize = 10)
        {
            ViewBag.SearchText = Search;
            ViewBag.PageNumber = PageNumber;
            ViewBag.PageSize = PageSize;

            var result = await _taskPriorityBL.TaskPriorityListAsync(
                Search,
                PageNumber,
                PageSize);

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePriority(TaskPriorityListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return await ReturnPriorityView(model.Entry, "Entry");
            }

            var result = await _taskPriorityBL.CreateTaskPriorityAsync(model.Entry);

            switch (result)
            {
                case 0:
                    TempData["SuccessMessage"] =
                        "Task Priority created successfully.";

                    return RedirectToAction(nameof(TaskPriorityList));

                case 1:
                    ModelState.AddModelError(
                        "Entry.Priority_Code",
                        "Task Priority Code already exists!"
                    );
                    break;

                case 2:
                    ModelState.AddModelError(
                        "Entry.Priority_Name",
                        "Priority Name already exists!"
                    );
                    break;

                case 3:
                    ModelState.AddModelError(
                        "Entry.SortOrder",
                        "Sort Order must be between 1 and 100."
                    );
                    break;

                default:
                    ModelState.AddModelError(
                        "",
                        "An error occurred while creating Task Priority."
                    );
                    break;
            }

            return await ReturnPriorityView(model.Entry, "Entry");
        }

        [HttpGet]
        public async Task<IActionResult> EditPriority(string Priority_Code)
        {
            if (string.IsNullOrWhiteSpace(Priority_Code))
            {
                TempData["ErrorMessage"] =
                    "Priority Code is required.";

                return RedirectToAction(nameof(TaskPriorityList));
            }

            var model =
                await _taskPriorityBL.GetTaskPriorityByCodeAsync(Priority_Code);

            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Task Priority not found.";

                return RedirectToAction(nameof(TaskPriorityList));
            }

            var listModel =
                await _taskPriorityBL.TaskPriorityListAsync(null, 1, 10);

            listModel.Entry = model;
            listModel.Entry.Mode = "Edit";

            return View("TaskPriorityList", listModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePriority(TaskPriorityListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return await ReturnPriorityView(model.Entry, "Edit");
            }

            var result = await _taskPriorityBL.UpdateTaskPriorityAsync(model.Entry);

            switch (result)
            {
                case 0:
                    TempData["SuccessMessage"] =
                        "Task Priority updated successfully.";

                    return RedirectToAction(nameof(TaskPriorityList));

                case 1:
                    ModelState.AddModelError(
                        "Entry.Priority_Code",
                        "Task Priority Code not found!"
                    );
                    break;

                case 2:
                    ModelState.AddModelError(
                        "Entry.Priority_Name",
                        "Priority Name already exists!"
                    );
                    break;

                case 3:
                    ModelState.AddModelError(
                        "Entry.SortOrder",
                        "Sort Order must be between 1 and 100."
                    );
                    break;

                default:
                    ModelState.AddModelError(
                        "",
                        "An error occurred while updating Task Priority."
                    );
                    break;
            }

            return await ReturnPriorityView(model.Entry, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePriorityList(string Priority_Code)
        {

            if (string.IsNullOrWhiteSpace(Priority_Code))
            {
                TempData["ErrorMessage"] = "Priority Code is Required!!";
                return RedirectToAction(nameof(TaskPriorityList));
            }

            var result = await _taskPriorityBL.DeleteTaskPriorityAsync(Priority_Code);

            if (result == 0)
            {
                TempData["SuccessMessage"] = "Task Priority deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Task Priority could not be deleted.";
            }

            return RedirectToAction(nameof(TaskPriorityList));
        }

        private async Task<IActionResult> ReturnPriorityView(
        CreateTaskPriorityViewModel model,string mode)
        {
            var listModel = await _taskPriorityBL.TaskPriorityListAsync(null, 1, 10);

            listModel.Entry = model;
            listModel.Entry.Mode = mode;

            ViewBag.OpenPriorityModal = true;

            return View("TaskPriorityList", listModel);
        }
    }
}
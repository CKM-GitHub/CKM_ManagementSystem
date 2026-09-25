using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CKM_ManagementSystem.Controllers
{
    public class TasksController : Controller
    {
        [HttpGet]
        public IActionResult TaskLists()
        {
            var model = new TasksViewModel();

            // want to Test ui so with temp data
            model.ProjectList = new List<SelectListItem>
            {
                new SelectListItem { Value = "PRJ001", Text = "Enterprise CRM" },
                new SelectListItem { Value = "PRJ002", Text = "E-Commerce App" }
            };

            model.PersonInChargeList = new List<SelectListItem>
            {
                new SelectListItem { Value = "STAFF01", Text = "John Doe" },
                new SelectListItem { Value = "STAFF02", Text = "Jane Smith" }
            };

            model.AssigneeList = new List<SelectListItem>
            {
                new SelectListItem { Value = "STAFF03", Text = "Sarah Smith" },
                new SelectListItem { Value = "STAFF04", Text = "Michael Brown" }
            };

            model.PriorityList = new List<SelectListItem>
            {
                new SelectListItem { Value = "LOW", Text = "Low" },
                new SelectListItem { Value = "MED", Text = "Medium" },
                new SelectListItem { Value = "HIGH", Text = "High" }
            };

            model.StatusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "OPEN", Text = "Open" },
                new SelectListItem { Value = "IN_PROGRESS", Text = "In Progress" },
                new SelectListItem { Value = "COMPLETED", Text = "Completed" }
            };

            model.TaskListData = new List<Tasks>
            {
                new Tasks
                {
                    No = 1,
                    ID = 101,
                    ProjectName = "Enterprise CRM",
                    PersonInCharge = "John Doe",
                    Title = "API Integration",
                    Description = "Integrate payment gateway API endpointsIntegrate payment gateway API endpointsIntegrate payment gateway API endpoints",
                    Assignee = "Sarah Smith",
                    IssueDate = DateTime.Now.AddDays(-10),
                    DueDate = DateTime.Now.AddDays(15),
                    Priority = "High",
                    Status = "In Progress",
                    StartDate = DateTime.Now.AddDays(-5),
                    TotalCount = 1
                }
            };

            model.TotalCount = model.TaskListData.Count;

            return View("~/Views/Tasks/TaskLists.cshtml", model);
        }
        [HttpPost]
        public IActionResult Search(TasksViewModel model)
        {
            model.ProjectList = new List<SelectListItem>
            {
                new SelectListItem { Value = "PRJ001", Text = "Enterprise CRM" },
                new SelectListItem { Value = "PRJ002", Text = "E-Commerce App" }
            };

            model.PersonInChargeList = new List<SelectListItem>
            {
                new SelectListItem { Value = "STAFF01", Text = "John Doe" },
                new SelectListItem { Value = "STAFF02", Text = "Jane Smith" }
            };

            model.AssigneeList = new List<SelectListItem>
            {
                new SelectListItem { Value = "STAFF03", Text = "Sarah Smith" },
                new SelectListItem { Value = "STAFF04", Text = "Michael Brown" }
            };

            model.PriorityList = new List<SelectListItem>
            {
                new SelectListItem { Value = "LOW", Text = "Low" },
                new SelectListItem { Value = "MED", Text = "Medium" },
                new SelectListItem { Value = "HIGH", Text = "High" }
            };

            model.StatusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "OPEN", Text = "Open" },
                new SelectListItem { Value = "IN_PROGRESS", Text = "In Progress" },
                new SelectListItem { Value = "COMPLETED", Text = "Completed" }
            };

            return View("~/Views/Tasks/TaskLists.cshtml", model);
        }
    }
}

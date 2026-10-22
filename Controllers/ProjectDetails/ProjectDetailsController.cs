using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using CKM_ManagementSystem.Models.ViewModels.ProjectDetails;
using CKM_ManagementSystem.BL;

namespace CKM_ManagementSystem.Controllers.Project
{
    [Route("ProjectDetails/[action]")]
    public class ProjectDetailsController : Controller
    {
        private readonly ProjectDetailsBL _projectBL;

        public ProjectDetailsController(ProjectDetailsBL projectBL)
        {
            _projectBL = projectBL;
        }

        [HttpGet]
        public async Task<IActionResult> ProjectDetailsLists(string? projectName)
        {
            string searchKeyword = projectName ?? string.Empty;

            DataTable projectList = await Task.Run(() => _projectBL.GetProjectListWithTaskCounts(searchKeyword));
            DataTable overview = await Task.Run(() => _projectBL.GetDashboardOverview(searchKeyword));

            var viewModel = new ProjectDetailsDashboardViewModel
            {
                SearchQuery = searchKeyword,
                Projects = new List<ProjectListItem>()
            };

            if (overview != null && overview.Rows.Count > 0)
            {
                DataRow row = overview.Rows[0];
                int GetVal(string colName)
                {
                    if (overview.Columns.Contains(colName) && row[colName] != DBNull.Value)
                    {
                        return Convert.ToInt32(row[colName]);
                    }
                    return 0;
                }

                viewModel.TotalOpenTasks = GetVal("TotalOpenTasks");
                viewModel.TotalInProgressTasks = GetVal("TotalInProgressTasks");
                viewModel.TotalReviewTasks = GetVal("TotalReviewTasks");
                viewModel.TotalCompletedTasks = GetVal("TotalCompletedTasks");
                viewModel.TotalOnHoldTasks = GetVal("TotalOnHoldTasks");
                viewModel.TotalCancelledTasks = GetVal("TotalCancelledTasks");

                viewModel.TotalActiveProjects = GetVal("TotalActiveProjects");
                viewModel.TotalOnHoldProjects = GetVal("TotalOnHoldProjects");
                viewModel.TotalCompletedProjects = GetVal("TotalCompletedProjects");
                viewModel.TotalInactiveProjects = GetVal("TotalInactiveProjects");
            }

            if (projectList != null && projectList.Rows.Count > 0)
            {
                foreach (DataRow row in projectList.Rows)
                {
                    string GetStr(string col) => projectList.Columns.Contains(col) && !row.IsNull(col) ? row[col]?.ToString() ?? string.Empty : string.Empty;
                    int GetInt(string col) => projectList.Columns.Contains(col) && row[col] != DBNull.Value ? Convert.ToInt32(row[col]) : 0;
                    DateTime? GetDate(string col) => projectList.Columns.Contains(col) && row[col] != DBNull.Value ? Convert.ToDateTime(row[col]) : (DateTime?)null;

                    viewModel.Projects.Add(new ProjectListItem
                    {
                        ProjectCode = GetStr("ProjectCode"),
                        ProjectName = GetStr("ProjectName"),
                        Status = GetStr("Status"),
                        Updated_Date = GetDate("Updated_Date"),
                        OpenCount = GetInt("OpenCount"),
                        InProgressCount = GetInt("InProgressCount"),
                        ReviewCount = GetInt("ReviewCount"),
                        CompletedCount = GetInt("CompletedCount"),
                        OnHoldCount = GetInt("OnHoldCount"),
                        CancelledCount = GetInt("CancelledCount")
                    });
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectDetails(string projectCode, int pageNumber = 1, int pageSize = 5)
        {
            if (string.IsNullOrWhiteSpace(projectCode))
            {
                return Json(new { success = false, message = "Project Code is required." });
            }
            var (projectInfo, projectMembers, totalMembers) = await Task.Run(() =>
                _projectBL.GetProjectDetailsByCode(projectCode, pageNumber, pageSize));

            if (projectInfo == null || projectInfo.Rows.Count == 0)
            {
                return Json(new { success = false, message = "Project details not found." });
            }

            var projectInfoList = projectInfo.AsEnumerable()
                .Select(row =>
                projectInfo.Columns.Cast<DataColumn>()
                .ToDictionary(
                    col => col.ColumnName, 
                    col => row[col] == DBNull.Value ? null : row[col]
                    )
            ).ToList();

            var projectMembersList = projectMembers.AsEnumerable()
                .Select(row =>
                projectMembers.Columns.Cast<DataColumn>()
                .ToDictionary(col => col.ColumnName, 
                col => row[col] == DBNull.Value ? null : row[col]
                )
            ).ToList();

            int totalPages = (int)Math.Ceiling((double)totalMembers / pageSize);

            return Json(new
            {
                success = true,
                projectInfo = projectInfoList,
                projectMembers = projectMembersList,
                currentPage = pageNumber,
                totalPages = totalPages,
                totalMembers = totalMembers, 
                pageSize = pageSize
            });
        }
    }
}
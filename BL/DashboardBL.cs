using System.Data;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.DL;
using Microsoft.Data.SqlClient;

namespace CKM_ManagementSystem.BL
{
    public class DashboardBL
    {
        private readonly BaseDL _bdl;

        public DashboardBL(BaseDL basedl)
        {
            _bdl = basedl;
        }

        public async Task<List<TaskStatusSummaryViewModel>> GetTaskStatusSummaryAsync(string staffCode)
        {
            DataTable dt = await _bdl.SelectDataTableAsync(
                "sp_GetTaskStatusSummary",
                new SqlParameter("@LoginUserID", staffCode)
                );

            var result = new List<TaskStatusSummaryViewModel>();

            foreach (DataRow row in dt.Rows)
            {
                result.Add(new TaskStatusSummaryViewModel
                {
                    StatusName = Convert.ToString(row["Status_Name"]) ?? string.Empty,
                    StatusCount = Convert.ToInt32(row["Status_Count"])
                });
            }

            return result;
        }

        public async Task<List<AnnouncementListViewModel>> GetAnnouncementListAsync(string staffCode)
        {
            DataTable dt = await _bdl.SelectDataTableAsync(
                "sp_GetAnnouncementList",
                new SqlParameter("@LoginUserID", staffCode)
                );

            var result = new List<AnnouncementListViewModel>();

            foreach(DataRow row in dt.Rows)
            {
                result.Add(new AnnouncementListViewModel
                {
                    ID = Convert.ToInt32(row["ID"]),
                    Title = Convert.ToString(row["Title"]) ?? string.Empty
                });
            }
            return result;
        }

        public async Task<List<ProjectTaskSummaryViewModel>> GetProjectTaskSummaryAsync(string staffCode)
        {
            DataTable dt = await _bdl.SelectDataTableAsync(
                "sp_GetProjectTaskSummary",
                new SqlParameter("@LoginUserID", staffCode)
                );

            var result = new List<ProjectTaskSummaryViewModel>();

            foreach (DataRow row in dt.Rows) 
            {
                result.Add(new ProjectTaskSummaryViewModel
                {
                    ProjectName = Convert.ToString(row["ProjectName"]) ?? string.Empty,
                    NewTasks = Convert.ToInt32(row["New_Tasks"])    ,
                    OngoingTasks = Convert.ToInt32(row["Ongoing_Tasks"]),
                    Status = Convert.ToString(row["Status"]) ?? string.Empty
                });
            }

            return result;
        }

        public async Task<AnnouncementDetailsViewModel?> GetAnnouncementDetailsAsync(int id)
        {
            DataTable dt = await _bdl.SelectDataTableAsync(
                "sp_AnnouncementDatils",
                new SqlParameter("@AnnouncementID", id)
                );

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dt.Rows[0];

            return new AnnouncementDetailsViewModel
            {
                Title = Convert.ToString(row["Title"]) ?? string.Empty,
                Name = Convert.ToString(row["Name"]) ?? string.Empty,
                DepartmentName = Convert.ToString(row["Department_Name"]) ?? string.Empty,
                Content = Convert.ToString(row["Content"]) ?? string.Empty
            };

        }

    }
}

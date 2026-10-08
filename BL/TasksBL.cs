using System.Data;
using System.Security.AccessControl;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Abstractions;

namespace CKM_ManagementSystem.BL
{
    public class TasksBL
    {
        private readonly BaseDL bdl;

        public TasksBL(BaseDL baseDL)
        {
            bdl = baseDL;
        }


        /*Common Methods*/

        public async Task<List<TaskLookupItemViewModel>>
          GetProjectsAsync()
        {
            DataTable table =
                await bdl.SelectDataTableAsync(
                    "sp_Task_GetProjects", null);

            var projects = new List<TaskLookupItemViewModel>();

            foreach (DataRow row in table.Rows)
            {
                projects.Add(
                    new TaskLookupItemViewModel
                    {
                        Value = row["ProjectCode"]?.ToString()
                        ?? string.Empty,

                        Text = row["ProjectName"]?.ToString()
                        ?? string.Empty
                    });
            }
            return projects;
        }
        public async Task<List<TaskLookupItemViewModel>>
            GetPrioritiesAsync()
        {
            DataTable table = await bdl.SelectDataTableAsync(
                              "sp_Task_GetPriorities",
                              null);
            var priorities = new List<TaskLookupItemViewModel>();

            foreach (DataRow row in table.Rows)
            {
                priorities.Add(
                       new TaskLookupItemViewModel
                       {
                           Value = row["Priority_Code"]?.ToString()
                           ?? string.Empty,

                           Text = row["Priority_Name"]?.ToString()
                           ?? string.Empty
                       });
            }
            return priorities;
        }
        public async Task<List<TaskLookupItemViewModel>>
            GetStatusesAsync()
        {
            DataTable table = await bdl.SelectDataTableAsync(
                         "sp_Task_GetStatuses",
                         null);

            var statuses = new List<TaskLookupItemViewModel>();

            foreach (DataRow row in table.Rows)
            {
                statuses.Add(
                    new TaskLookupItemViewModel
                    {
                        Value = row["Status_Code"]?.ToString()
                     ?? string.Empty,

                        Text = row["Status_Name"]?.ToString()
                     ?? string.Empty

                    });
            }
            return statuses;
        }

        /*Task_Entry*/
        public async Task<List<TaskLookupItemViewModel>>
            GetAssigneesAsync(string projectCode)
        {
            if (string.IsNullOrWhiteSpace(projectCode))
            {
                return new List<TaskLookupItemViewModel>();
            }

            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@ProjectCode",
                    projectCode
                    )
            };

            DataTable table = await bdl.SelectDataTableAsync(
                "sp_Task_GetAssignees",
                parameters);

            var assignees = new List<TaskLookupItemViewModel>();

            foreach (DataRow row in table.Rows)
            {
                assignees.Add(
                    new TaskLookupItemViewModel
                    {
                        Value = row["Staff_Code"]?.ToString()
                        ?? string.Empty,

                        Text = row["Name"]?.ToString()
                        ?? string.Empty
                    });
            }
            return assignees;
        }

        public async Task<int> CreateTaskAsync(
            TaskCreateViewModel model,
            string loginStaffCode)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
                    "@ProjectCode",
                    model.ProjectCode),

                new SqlParameter(
                    "@PersonInCharge",
                    loginStaffCode),

                new SqlParameter(
                    "@Title",
                    model.Title),

                new SqlParameter(
                    "@Description",
                    string.IsNullOrWhiteSpace(model.Description)
                    ? DBNull.Value : model.Description
                    ),

                new SqlParameter(
                    "@Assignee",
                    model.Assignee),

                new SqlParameter(
                    "@DueDate",
                    model.DueDate.HasValue
                    ? model.DueDate.Value
                    : DBNull.Value),

                new SqlParameter(
                    "@Priority_Code",
                    model.PriorityCode),

                new SqlParameter(
                    "@Status_Code",
                    model.StatusCode),

                new SqlParameter(
                    "@Attachments",
                    string.IsNullOrWhiteSpace(model.Attachements)
                    ? DBNull.Value
                    : model.Attachements),

                new SqlParameter(
                    "@CreateBy",
                    loginStaffCode)

            };

            DataTable table = await bdl.SelectDataTableAsync(
                "sp_Task_Create",
                parameters);

            if (table.Rows.Count == 0)
            {
                return 0;
            }

            return Convert.ToInt32(
                table.Rows[0]["TaskID"]);
        }

        
        // Task Management
        

        public async Task<List<TaskLookupItemViewModel>>
            GetPersonsInChargeAsync()
        {
            DataTable table =
                await bdl.SelectDataTableAsync(
                    "sp_Task_GetPersonInCharge",
                    null
                );

            var personsInCharge =
                new List<TaskLookupItemViewModel>();

            foreach (DataRow row in table.Rows)
            {
                personsInCharge.Add(
                    new TaskLookupItemViewModel
                    {
                        Value =
                            row["Staff_Code"]?.ToString()
                            ?? string.Empty,

                        Text =
                            row["Name"]?.ToString()
                            ?? string.Empty
                    }
                );
            }

            return personsInCharge;
        }

        public async Task<List<TaskLookupItemViewModel>>
            GetManagementAssigneesAsync()
        {
            DataTable table =
                await bdl.SelectDataTableAsync(
                    "sp_Task_GetManagementAssignees",
                    null
                );

            var assignees =
                new List<TaskLookupItemViewModel>();

            foreach (DataRow row in table.Rows)
            {
                assignees.Add(
                    new TaskLookupItemViewModel
                    {
                        Value =
                            row["Staff_Code"]?.ToString()
                            ?? string.Empty,

                        Text =
                            row["Name"]?.ToString()
                            ?? string.Empty
                    }
                );
            }

            return assignees;
        }

        public async Task<List<TaskManagementListItemViewModel>>
            GetTaskManagementListAsync(
                TaskManagementFilterViewModel filter
            )
        {
            SqlParameter[] parameters =
            {
        new SqlParameter(
            "@ProjectCode",
            string.IsNullOrWhiteSpace(filter.ProjectCode)
                ? DBNull.Value
                : filter.ProjectCode
        ),

        new SqlParameter(
            "@PersonInCharge",
            string.IsNullOrWhiteSpace(filter.PersonInCharge)
                ? DBNull.Value
                : filter.PersonInCharge
        ),

        new SqlParameter(
            "@Title",
            string.IsNullOrWhiteSpace(filter.Title)
                ? DBNull.Value
                : filter.Title
        ),

        new SqlParameter(
            "@IssueDateStart",
            filter.IssueDateStart.HasValue
                ? filter.IssueDateStart.Value
                : DBNull.Value
        ),

        new SqlParameter(
            "@IssueDateEnd",
            filter.IssueDateEnd.HasValue
                ? filter.IssueDateEnd.Value
                : DBNull.Value
        ),

        new SqlParameter(
            "@DueDateStart",
            filter.DueDateStart.HasValue
                ? filter.DueDateStart.Value
                : DBNull.Value
        ),

        new SqlParameter(
            "@DueDateEnd",
            filter.DueDateEnd.HasValue
                ? filter.DueDateEnd.Value
                : DBNull.Value
        ),

        new SqlParameter(
            "@Assignee",
            string.IsNullOrWhiteSpace(filter.Assignee)
                ? DBNull.Value
                : filter.Assignee
        ),

        new SqlParameter(
            "@Priority_Code",
            string.IsNullOrWhiteSpace(filter.PriorityCode)
                ? DBNull.Value
                : filter.PriorityCode
        ),

        new SqlParameter(
            "@Status_Code",
            string.IsNullOrWhiteSpace(filter.StatusCode)
                ? DBNull.Value
                : filter.StatusCode
        )
    };

            DataTable table =
                await bdl.SelectDataTableAsync(
                    "sp_Task_Management_List",
                    parameters
                );

            var tasks =
                new List<TaskManagementListItemViewModel>();

            foreach (DataRow row in table.Rows)
            {
                tasks.Add(
                    new TaskManagementListItemViewModel
                    {
                        ID = Convert.ToInt32(row["ID"]),

                        ProjectName =
                            row["ProjectName"]?.ToString()
                            ?? string.Empty,

                        PersonInChargeName =
                            row["PersonInChargeName"]?.ToString()
                            ?? string.Empty,

                        Title =
                            row["Title"]?.ToString()
                            ?? string.Empty,

                        Description =
                            row["Description"] == DBNull.Value
                                ? null
                                : row["Description"]?.ToString(),

                        AssigneeName =
                            row["AssigneeName"]?.ToString()
                            ?? string.Empty,

                        IssueDate =
                            Convert.ToDateTime(row["IssueDate"]),

                        DueDate =
                            row["DueDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(row["DueDate"]),

                        PriorityName =
                            row["Priority_Name"]?.ToString()
                            ?? string.Empty,

                        StatusName =
                            row["Status_Name"]?.ToString()
                            ?? string.Empty,

                        StartDate =
                            row["StartDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(row["StartDate"]),

                        EndDate =
                            row["EndDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(row["EndDate"]),

                        Attachments =
                            row["Attachments"] == DBNull.Value
                                ? null
                                : row["Attachments"]?.ToString()
                    }
                );
            }

            return tasks;
        }

        public async Task<bool> UpdateTaskAsync(
            TaskManagementUpdateViewModel model,
            string loginStaffCode
        )
        {
            SqlParameter[] parameters =
            {
        new SqlParameter(
            "@ID",
            model.ID
        ),

        new SqlParameter(
            "@Title",
            model.Title
        ),

        new SqlParameter(
            "@Description",
            string.IsNullOrWhiteSpace(model.Description)
                ? DBNull.Value
                : model.Description
        ),

        new SqlParameter(
            "@Assignee",
            model.Assignee
        ),

        new SqlParameter(
            "@DueDate",
            model.DueDate.HasValue
                ? model.DueDate.Value
                : DBNull.Value
        ),

        new SqlParameter(
            "@Priority_Code",
            model.PriorityCode
        ),

        new SqlParameter(
            "@Status_Code",
            model.StatusCode
        ),

        new SqlParameter(
            "@Attachments",
            string.IsNullOrWhiteSpace(model.Attachments)
                ? DBNull.Value
                : model.Attachments
        ),

        new SqlParameter(
            "@UpdatedBy",
            loginStaffCode
        )
    };

            DataTable table =
                await bdl.SelectDataTableAsync(
                    "sp_Task_Management_Update",
                    parameters
                );

            if (table.Rows.Count == 0)
            {
                return false;
            }

            int affectedRows =
                Convert.ToInt32(
                    table.Rows[0]["AffectedRows"]
                );

            return affectedRows > 0;
        }
        
        // My Task Overview
      
        public async Task<List<MyTaskListItemViewModel>>
            GetMyTaskListAsync(
                MyTaskFilterViewModel filter,
                string loginStaffCode
            )
        {
            SqlParameter[] parameters =
            {
        new SqlParameter(
            "@LoginStaffCode",
            loginStaffCode
        ),

        new SqlParameter(
            "@ProjectCode",
            string.IsNullOrWhiteSpace(filter.ProjectCode)
                ? DBNull.Value
                : filter.ProjectCode
        ),

        new SqlParameter(
            "@Title",
            string.IsNullOrWhiteSpace(filter.Title)
                ? DBNull.Value
                : filter.Title
        ),

        new SqlParameter(
            "@DueDateStart",
            filter.DueDateStart.HasValue
                ? filter.DueDateStart.Value
                : DBNull.Value
        ),

        new SqlParameter(
            "@DueDateEnd",
            filter.DueDateEnd.HasValue
                ? filter.DueDateEnd.Value
                : DBNull.Value
        ),

        new SqlParameter(
            "@Priority_Code",
            string.IsNullOrWhiteSpace(filter.PriorityCode)
                ? DBNull.Value
                : filter.PriorityCode
        ),

        new SqlParameter(
            "@Status_Code",
            string.IsNullOrWhiteSpace(filter.StatusCode)
                ? DBNull.Value
                : filter.StatusCode
        )
    };

            DataTable table =
                await bdl.SelectDataTableAsync(
                    "sp_MyTask_List",
                    parameters
                );

            var tasks =
                new List<MyTaskListItemViewModel>();

            foreach (DataRow row in table.Rows)
            {
                tasks.Add(
                    new MyTaskListItemViewModel
                    {
                        ID =
                            Convert.ToInt32(row["ID"]),

                        ProjectName =
                            row["ProjectName"]?.ToString()
                            ?? string.Empty,

                        PersonInChargeName =
                            row["PersonInChargeName"]?.ToString()
                            ?? string.Empty,

                        Title =
                            row["Title"]?.ToString()
                            ?? string.Empty,

                        Description =
                            row["Description"] == DBNull.Value
                                ? null
                                : row["Description"]?.ToString(),

                        AssigneeName =
                            row["AssigneeName"]?.ToString()
                            ?? string.Empty,

                        IssueDate =
                            Convert.ToDateTime(
                                row["IssueDate"]
                            ),

                        DueDate =
                            row["DueDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(
                                    row["DueDate"]
                                ),

                        PriorityName =
                            row["Priority_Name"]?.ToString()
                            ?? string.Empty,

                        StatusName =
                            row["Status_Name"]?.ToString()
                            ?? string.Empty,

                        StartDate =
                            row["StartDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(
                                    row["StartDate"]
                                ),

                        EndDate =
                            row["EndDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(
                                    row["EndDate"]
                                ),

                        Attachments =
                            row["Attachments"] == DBNull.Value
                                ? null
                                : row["Attachments"]?.ToString()
                    }
                );
            }

            return tasks;
        }

        public async Task<List<MyTaskStatusSummaryViewModel>>
            GetMyTaskStatusSummaryAsync(
                string loginStaffCode
            )
        {
            SqlParameter[] parameters =
            {
        new SqlParameter(
            "@LoginStaffCode",
            loginStaffCode
        )
    };

            DataTable table =
                await bdl.SelectDataTableAsync(
                    "sp_MyTask_StatusSummary",
                    parameters
                );

            var summaries =
                new List<MyTaskStatusSummaryViewModel>();

            foreach (DataRow row in table.Rows)
            {
                summaries.Add(
                    new MyTaskStatusSummaryViewModel
                    {
                        StatusCode =
                            row["Status_Code"]?.ToString()
                            ?? string.Empty,

                        StatusName =
                            row["Status_Name"]?.ToString()
                            ?? string.Empty,

                        StatusCount =
                            Convert.ToInt32(
                                row["Status_Count"]
                            ),

                        TotalTasks =
                            Convert.ToInt32(
                                row["Total_Tasks"]
                            )
                    }
                );
            }

            return summaries;
        }

        public async Task<bool> UpdateMyTaskAsync(
            MyTaskUpdateViewModel model,
            string loginStaffCode
        )
        {
            SqlParameter[] parameters =
            {
        new SqlParameter(
            "@ID",
            model.ID
        ),

        new SqlParameter(
            "@Status_Code",
            model.StatusCode
        ),

        new SqlParameter(
            "@StartDate",
            model.StartDate.HasValue
                ? model.StartDate.Value
                : DBNull.Value
        ),

        new SqlParameter(
            "@EndDate",
            model.EndDate.HasValue
                ? model.EndDate.Value
                : DBNull.Value
        ),

        new SqlParameter(
            "@LoginStaffCode",
            loginStaffCode
        )
    };

            DataTable table =
                await bdl.SelectDataTableAsync(
                    "sp_MyTask_Update",
                    parameters
                );

            if (table.Rows.Count == 0)
            {
                return false;
            }

            int affectedRows =
                Convert.ToInt32(
                    table.Rows[0]["AffectedRows"]
                );

            return affectedRows > 0;
        }
    }
}
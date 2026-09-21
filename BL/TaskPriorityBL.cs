using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models;
using CKM_ManagementSystem.Models.ViewModels.Common;
using CKM_ManagementSystem.Models.ViewModels.Task;
using CKM_ManagementSystem.Models.ViewModels.TaskPriorities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CKM_ManagementSystem.BL
{
    public class TaskPriorityBL
    {
        private readonly BaseDL _bdl;

        public TaskPriorityBL(BaseDL bdl)
        {
            _bdl = bdl;
        }
        public async Task<TaskPriorityListViewModel> TaskPriorityListAsync(
            string? Search,
            int PageNumber = 1,
            int PageSize = 6,
            int TotalCount = 0)
        {
            if(PageNumber < 1)
            {
                PageNumber = 1;
            }
            if(PageSize < 1 || PageSize > 100)
            {
                PageSize = 10;
            }

            if(string.IsNullOrWhiteSpace(Search))
            {
                Search = null;
            }

            var taskPriorityList = new List<TaskPriorityListItemViewModel>();

            SqlParameter[] parameters = 
            {
                new SqlParameter("@Search", Search ?? (object)DBNull.Value),
                new SqlParameter("@PageNumber", PageNumber),
                new SqlParameter("@PageSize", PageSize)
            };

            var result = await _bdl.SelectDataTableAsync(
                "sp_Task_Priorities_List",
                parameters);

            if (result.Rows.Count > 0)
            {
                foreach (DataRow row in result.Rows)
                {
                    taskPriorityList.Add(new TaskPriorityListItemViewModel
                    {
                        Priority_Code = row["Priority_Code"]?.ToString() ?? string.Empty,
                        Priority_Name = row["Priority_Name"]?.ToString() ?? string.Empty,
                        Description = row["Description"]?.ToString() ?? string.Empty,
                        SortOrder = row["SortOrder"] != DBNull.Value
                            ? Convert.ToInt32(row["SortOrder"])
                            : 0
                    });

                    if (TotalCount == 0 && row["TotalCount"] != DBNull.Value)
                    {
                        TotalCount = Convert.ToInt32(row["TotalCount"]);
                    }
                }
            }
            var pagedData = new PagedResponse<TaskPriorityListItemViewModel>
            {
                Data = taskPriorityList,
                TotalCount = TotalCount,
                PageNumber = PageNumber,
                PageSize = PageSize
            };

            var response = new TaskPriorityListViewModel
            {
                PagedData = pagedData,
                Search = Search
            };

            return response;
        }

        public async Task<int> CreateTaskPriorityAsync(CreateTaskPriorityViewModel model) 
        {
            if (model == null)
                return 3;

            var parameters = new[]
            {
                new SqlParameter("@Priority_Code", SqlDbType.NVarChar, 20) { Value = model.Priority_Code },
                new SqlParameter("@Priority_Name", SqlDbType.NVarChar, 100) { Value = model.Priority_Name },
                new SqlParameter("@Description", SqlDbType.NVarChar, 500) { Value = (object?)model.Description ?? DBNull.Value },
                new SqlParameter("@SortOrder", SqlDbType.Int) { Value = model.SortOrder },
                new SqlParameter("@Error_Code", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                }
            };
            return await _bdl.ExecuteNonQueryWithErrorCodeAsync("sp_Create_TaskPriorities", parameters);
        }

        public async Task<CreateTaskPriorityViewModel> GetTaskPriorityByCodeAsync(string priorityCode)
        {
            if (string.IsNullOrWhiteSpace(priorityCode))
                return null;

            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@Priority_Code", priorityCode)
            };
            var result = await _bdl.SelectDataTableAsync(
                "sp_TaskPriorities_GetByCode",
                parameters);

            if (result.Rows.Count > 0)
            {
                var row = result.Rows[0];
                return new CreateTaskPriorityViewModel
                {
                    Priority_Code = row["Priority_Code"]?.ToString() ?? string.Empty,
                    Priority_Name = row["Priority_Name"]?.ToString() ?? string.Empty,
                    Description = row["Description"]?.ToString() ?? string.Empty,
                    SortOrder = row["SortOrder"] != DBNull.Value
                        ? Convert.ToInt32(row["SortOrder"])
                        : 0
                };
            }
            return null;
        }   

        public async Task<int> UpdateTaskPriorityAsync(CreateTaskPriorityViewModel model)
        {
            if (model == null)
                return 3;

            var parameters = new[]
            {
                new SqlParameter("@Priority_Code", SqlDbType.NVarChar, 20) { Value = model.Priority_Code },
                new SqlParameter("@Priority_Name", SqlDbType.NVarChar, 100) { Value = model.Priority_Name },
                new SqlParameter("@Description", SqlDbType.NVarChar, 500) { Value = (object?)model.Description ?? DBNull.Value },
                new SqlParameter("@SortOrder", SqlDbType.Int) { Value = model.SortOrder },
                new SqlParameter("@Error_Code", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                }
            };
            return await _bdl.ExecuteNonQueryWithErrorCodeAsync(
                "sp_Update_TaskPriorities",
                parameters);
        }
        public async Task<int> DeleteTaskPriorityAsync(string priorityCode)
        {
            if (string.IsNullOrWhiteSpace(priorityCode))
                return 0;

            var parameters = new[]
            {
        new SqlParameter("@Priority_Code", SqlDbType.NVarChar, 20)
        {
            Value = priorityCode
        },
        new SqlParameter("@Error_Code", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        }
    };

            return await _bdl.ExecuteNonQueryWithErrorCodeAsync(
                "sp_Delete_TaskPriorities",
                parameters);
        }
    }
}

using System.Data;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.ViewModels.TaskStatuses;
using Microsoft.Data.SqlClient;
using CKM_ManagementSystem.Models.ViewModels.Common;


namespace CKM_ManagementSystem.BL
{
    public class TaskStatusesBL :BaseDL
    {
        public TaskStatusesBL(IConfiguration configuration):base(configuration) { }
        
        public async Task<CreateTaskStatusesViewModel?> GetTaskStatusByCodeAsync(string statusCode)
        {
            if (string.IsNullOrWhiteSpace(statusCode))
            {
                return null;
            }
            SqlParameter[] parameters = { new SqlParameter("@Status_Code", statusCode) };

            var result = await SelectDataTableAsync("sp_GetTaskStatusByCode", parameters);

            if(result.Rows.Count == 0) { return null;}
            DataRow row = result.Rows[0];
            return new CreateTaskStatusesViewModel
            {
                Status_Code = row["Status_Code"]?.ToString() ?? string.Empty,
                Status_Name = row["Status_Name"]?.ToString() ?? string.Empty,
                Description = row["Description"]?.ToString() ?? string.Empty,
                SortOrder = row["SortOrder"] != DBNull.Value
                    ? Convert.ToInt32(row["SortOrder"])
                    : 0
            };
        }
        
        public async Task<TaskStatusesPagesResultViewModel> GetTaskStatusesListAsync(
            string? Search = null,
            int PageNumber = 1,
            int PageSize = 10,
            int TotalCount = 0)
        {
            if (PageNumber < 1) { PageNumber = 1; }
            if (PageSize < 1 || PageSize > 100) { PageSize = 10; }
            if (string.IsNullOrWhiteSpace(Search))
            {
                Search = null;
            }
            SqlParameter[] parameters =
            {
                new SqlParameter("@Search", Search ?? (object)DBNull.Value),
                new SqlParameter("@PageNumber", PageNumber),
                new SqlParameter("@PageSize", PageSize)
            };
            var result = await SelectDataTableAsync("sp_Task_Statuses_List", parameters);
            if (result.Rows.Count > 0 && result.Rows[0]["TotalCount"] != DBNull.Value)
            {
                TotalCount = Convert.ToInt32(result.Rows[0]["TotalCount"]);
            }
            if (result.Rows.Count == 0 && PageNumber > 1)
            {
                parameters = new[] 
                {
                    new SqlParameter("@Search", Search ?? (object)DBNull.Value),
                    new SqlParameter("@PageNumber", 1),
                    new SqlParameter("@PageSize", PageSize)
                };
                var firstPageResult = await SelectDataTableAsync("sp_Task_Statuses_List", parameters);
                if (firstPageResult.Rows.Count > 0 && firstPageResult.Rows[0]["TotalCount"] != DBNull.Value)
                {
                    TotalCount = Convert.ToInt32(firstPageResult.Rows[0]["TotalCount"]);
                }

                int totalPages = (int)Math.Ceiling((double)TotalCount / PageSize);
                PageNumber = totalPages > 0 ? totalPages : 1;
                parameters = new[]
                {
                    new SqlParameter("@Search", Search ?? (object)DBNull.Value),
                    new SqlParameter("@PageNumber", PageNumber),
                    new SqlParameter("@PageSize", PageSize)
                };
                result = await SelectDataTableAsync("sp_Task_Statuses_List", parameters);
            }
            var taskStatusList = new List<TaskStatusesListViewModel>();
            if(result.Rows.Count > 0)
            {
                foreach (DataRow row in result.Rows)
                {
                    taskStatusList.Add(new TaskStatusesListViewModel
                    {
                        Status_Code = row["Status_Code"]?.ToString() ?? string.Empty,
                        Status_Name = row["Status_Name"]?.ToString() ?? string.Empty,
                        Description = row["Description"]?.ToString() ?? string.Empty,
                        SortOrder = row["SortOrder"] != DBNull.Value ? Convert.ToInt32(row["SortOrder"]) : 0,
                        Created_Date = row["Created_Date"] != DBNull.Value ? Convert.ToDateTime(row["Created_Date"]) : null,
                        Updated_Date = row["Updated_Date"] != DBNull.Value ? Convert.ToDateTime(row["Updated_Date"]) : null
                    });
                    if (TotalCount == 0 && row["TotalCount"] != DBNull.Value)
                    {
                        TotalCount = Convert.ToInt32(row["TotalCount"]);
                    }
                }
            }
            var pagedData = new PagedResponse<TaskStatusesListViewModel>
            {
                Data = taskStatusList,
                TotalCount = TotalCount,
                PageNumber = PageNumber,
                PageSize = PageSize
            };
            var response = new TaskStatusesPagesResultViewModel
            {
                PagedData = pagedData,
                Search = Search
            };
            return response;
        }
        public async Task<TaskStatusesResult> CreateTaskStatusesAsync(
            string? statusCode,
            string? statusName,
            string? description,
            int sortOrder)
        {
            var validationError = ValidateTaskStatusesInput(statusCode, statusName);
            if(validationError != null) { return validationError; }
            var (responseCodeParam, responseMessageParam) = CreateOutputParameters();
            var parameters = new[]
            {
                new SqlParameter("@Status_Code" , (object?)statusCode ?? DBNull.Value),
                new SqlParameter("@Status_Name", (object?)statusName ?? DBNull.Value),
                new SqlParameter("@Description", (object?)description ?? DBNull.Value),
                new SqlParameter("@SortOrder", sortOrder),
                responseCodeParam,
                responseMessageParam
            };
            await ExecuteAsync("sp_CreateTaskStatuses", parameters);
            return ParseActionResult(responseCodeParam, responseMessageParam);
        }

       public async Task<TaskStatusesResult> UpdateTaskStatusesAsync(
           string? statusCode,
           string? statusName,
           string? description,
           int sortOrder)
        {
            var validationError = ValidateTaskStatusesInput(statusCode, statusName);
            if(validationError != null) return validationError;
            var (responseCodeParam, responseMessageParam) = CreateOutputParameters();
            var parameters = new[]
            {
                new SqlParameter("@Status_Code", (object?)statusCode ?? DBNull.Value),
                new SqlParameter("@Status_Name", (object?)statusName ?? DBNull.Value),
                new SqlParameter("@Description", (object?)description ?? DBNull.Value),
                new SqlParameter("@SortOrder", sortOrder),
                responseCodeParam,
                responseMessageParam
            };
            await ExecuteAsync("sp_UpdateTaskStatuses", parameters);
            return ParseActionResult(responseCodeParam, responseMessageParam);
        }

        public async Task<TaskStatusesResult> DeleteTaskStatusesAsync(string? statusCode)
        {
            var(responseCodeParam, responseMessageParam) = CreateOutputParameters();
            SqlParameter[] parameters =
            {
                new SqlParameter("@Status_Code", statusCode),
                responseCodeParam,
                responseMessageParam
            };
            await ExecuteAsync("sp_DeleteTaskStatuses", parameters);
            return ParseActionResult(responseCodeParam, responseMessageParam);
        }

        private TaskStatusesResult? ValidateTaskStatusesInput(string? statusCode, string? statusName)
        {
            if (string.IsNullOrWhiteSpace(statusCode)) return InvalidResult("Status code is required.");
            if (string.IsNullOrWhiteSpace(statusName)) return InvalidResult("Status name is required.");
            return null;
        }
        
        private TaskStatusesResult InvalidResult(string message)
        {
            return new TaskStatusesResult
            {
                ResponseCode = -1,
                ResponseMessage = message
            };
        }

        private (SqlParameter responseCode, SqlParameter responseMessage) CreateOutputParameters()
        {
            return (
                new SqlParameter("@ResponseCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                },
                new SqlParameter("@ResponseMessage", SqlDbType.NVarChar, 250)
                {
                    Direction = ParameterDirection.Output
                }
            );
        }

        private TaskStatusesResult ParseActionResult(SqlParameter statusCodeParam, SqlParameter statusMessageParam)
        {
            int responseCode = (statusCodeParam.Value != DBNull.Value) ? Convert.ToInt32(statusCodeParam.Value) : -1;
            string responseMessage = (statusMessageParam.Value != DBNull.Value) ? statusMessageParam.Value.ToString()! : "Unknown Status";
            return new TaskStatusesResult { ResponseCode = responseCode, ResponseMessage = responseMessage };
        }
        public class TaskStatusesResult
        {
            public int ResponseCode { get; set; }
            public string ResponseMessage { get; set; } = string.Empty;
        }
    }
}

using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.DL;

public class UserListDL : BaseDL, IUserListDL
{
    public UserListDL(IConfiguration configuration) : base(configuration) { }

    public Task<bool> DeleteUserAsync(string saffCode)
    {
        throw new NotImplementedException();
    }

    public async Task<(List<UserListViewModel> Users, int ErrorCode)> GetUsersAsync(
        string? searchText = null,
        bool? status = null,
        string? departmentCode = null,
        string? roleCode = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        List<UserListViewModel> users = new List<UserListViewModel>();

        using SqlConnection conn = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand("sp_GetUserList", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.Add(CreateParameter("@SearchText", searchText));
        cmd.Parameters.Add(CreateParameter("@Status", status));
        cmd.Parameters.Add(CreateParameter("@Department_Code", departmentCode));
        cmd.Parameters.Add(CreateParameter("@Role_Code", roleCode));
        cmd.Parameters.Add(CreateParameter("@PageNumber", pageNumber));
        cmd.Parameters.Add(CreateParameter("@PageSize", pageSize));

        SqlParameter errorParameter = new SqlParameter("@ErrorCode", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(errorParameter);

        await conn.OpenAsync();

        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
        {
            int staffCodeIdx = reader.GetOrdinal("Staff_Code");
            int nameIdx = reader.GetOrdinal("Name");
            int emailIdx = reader.GetOrdinal("Email");
            int imageUrlIdx = reader.GetOrdinal("Image_URL");
            int deptNameIdx = reader.GetOrdinal("Department_Name");
            int roleNameIdx = reader.GetOrdinal("Role_Name");
            int statusIdx = reader.GetOrdinal("Status");
            int overallTotalIdx = reader.GetOrdinal("OverallTotalCount");
            int overallActiveIdx = reader.GetOrdinal("OverallActiveCount");
            int overallInactiveIdx = reader.GetOrdinal("OverallInactiveCount");
            int totalCountIdx = reader.GetOrdinal("TotalCount");

            while (await reader.ReadAsync())
            {
                users.Add(new UserListViewModel
                {
                    StaffCode = reader.IsDBNull(staffCodeIdx) ? string.Empty : reader.GetString(staffCodeIdx),
                    Name = reader.IsDBNull(nameIdx) ? string.Empty : reader.GetString(nameIdx),
                    Email = reader.IsDBNull(emailIdx) ? string.Empty : reader.GetString(emailIdx),
                    ImageUrl = reader.IsDBNull(imageUrlIdx) ? string.Empty : reader.GetString(imageUrlIdx),
                    DepartmentName = reader.IsDBNull(deptNameIdx) ? string.Empty : reader.GetString(deptNameIdx),
                    RoleName = reader.IsDBNull(roleNameIdx) ? string.Empty : reader.GetString(roleNameIdx),
                    Status = !reader.IsDBNull(statusIdx) && reader.GetBoolean(statusIdx),
                    OverallTotalCount = reader.IsDBNull(overallTotalIdx) ? 0 : reader.GetInt32(overallTotalIdx),
                    OverallActiveCount = reader.IsDBNull(overallActiveIdx) ? 0 : reader.GetInt32(overallActiveIdx),
                    OverallInactiveCount = reader.IsDBNull(overallInactiveIdx) ? 0 : reader.GetInt32(overallInactiveIdx),
                    TotalCount = reader.IsDBNull(totalCountIdx) ? 0 : reader.GetInt32(totalCountIdx)
                });
            }
        }

        int errorCode = errorParameter.Value != DBNull.Value ? Convert.ToInt32(errorParameter.Value) : 0;

        return (users, errorCode);
    }
}

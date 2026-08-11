using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

public class UserListDL : BaseDL, IUserListDL
{
    public UserListDL(IConfiguration configuration) : base(configuration) { }

    public Task<bool> DeleteUserAsync(string saffCode)
    {
        throw new NotImplementedException();
    }

    public async Task<(List<UserListViewModel> Users,List<DepartmentDropdownViewModel> Departments , List<RoleDropdownViewModel> Roles, int ErrorCode)> GetUsersAsync(
        string? searchText = null,
        bool? status = null,
        string? departmentCode = null,
        string? roleCode = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        List<UserListViewModel> users = new List<UserListViewModel>();
        List<DepartmentDropdownViewModel> departments = new List<DepartmentDropdownViewModel>();
        List<RoleDropdownViewModel> roles = new List<RoleDropdownViewModel>();

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
            if (reader.HasRows)
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
                int deptCountIdx = reader.GetOrdinal("DepartmentCount");

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
                        TotalCount = reader.IsDBNull(totalCountIdx) ? 0 : reader.GetInt32(totalCountIdx),
                        DepartmentCount = reader.IsDBNull(deptCountIdx) ? 0 : reader.GetInt32(deptCountIdx)
                    });
                }
            }

            if (await reader.NextResultAsync())
            {
                int deptFCodeIdx = reader.GetOrdinal("Department_Code");  // F for filter
                int deptFNameIdx = reader.GetOrdinal("Department_Name");

                while (await reader.ReadAsync())
                {
                    departments.Add(new DepartmentDropdownViewModel
                    {
                        DepartmentCode = reader.IsDBNull(deptFCodeIdx) ? string.Empty : reader.GetString(deptFCodeIdx),
                        DepartmentName = reader.IsDBNull(deptFNameIdx) ? string.Empty : reader.GetString(deptFNameIdx)
                    });
                }
            }
            if (await reader.NextResultAsync())
            {
                int roleFCodeIdx = reader.GetOrdinal("Role_Code");   // F for filter
                int roleFNameIdx = reader.GetOrdinal("Role_Name");

                while (await reader.ReadAsync())
                {    
                    roles.Add(new RoleDropdownViewModel
                    {
                        RoleCode = reader.IsDBNull(roleFCodeIdx) ? string.Empty : reader.GetString(roleFNameIdx),
                        RoleName = reader.IsDBNull(roleFCodeIdx) ? string.Empty : reader.GetString(roleFNameIdx)
                    });
                }
            }
        }

        int errorCode = errorParameter.Value != DBNull.Value ? Convert.ToInt32(errorParameter.Value) : 0;

        return (users,departments,roles, errorCode);
    }
}

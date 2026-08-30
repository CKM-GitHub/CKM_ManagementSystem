using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CKM_ManagementSystem.BL
{
    public class UserListBL
    {
        private readonly BaseDL _bdl;

        public UserListBL(BaseDL baseDL)
        {
            _bdl = baseDL;
        }
        public async Task<PagedResponse<UserListViewModel>> GetUserListAsync(
            string? searchText,
            bool? status,
            string? departmentCode,
            string? roleCode,
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1 || pageSize > 100)
                pageSize = 10;

            SqlParameter[] parameters =
            {
                new SqlParameter("@SearchText", (object?)searchText ?? DBNull.Value),
                new SqlParameter("@Status", (object?)status ?? DBNull.Value),
                new SqlParameter("@Department_Code", (object?)departmentCode ?? DBNull.Value),
                new SqlParameter("@Role_Code", (object?)roleCode ?? DBNull.Value),
                new SqlParameter("@PageNumber", pageNumber),
                new SqlParameter("@PageSize", pageSize)
            };

            var errorParam = new SqlParameter("@ErrorCode", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            parameters = parameters.Append(errorParam).ToArray();

            var (
                overallTotalCount,
                overallActiveCount,
                overallInactiveCount,
                totalCount,
                departmentCount,
                users,
                departments,
                roles
            ) =
            await _bdl.ExecuteMultiResultReaderAsync(
                "sp_GetUserList",
                async reader =>
                {
                    int overallTotalCount = 0;
                    int overallActiveCount = 0;
                    int overallInactiveCount = 0;
                    int totalCount = 0;
                    int departmentCount = 0;

                    if (await reader.ReadAsync())
                    {
                        overallTotalCount = reader.GetInt32(reader.GetOrdinal("OverallTotalCount"));
                        overallActiveCount = reader.GetInt32(reader.GetOrdinal("OverallActiveCount"));
                        overallInactiveCount = reader.GetInt32(reader.GetOrdinal("OverallInactiveCount"));
                        totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
                        departmentCount = reader.GetInt32(reader.GetOrdinal("DepartmentCount"));
                    }

                    var users = new List<UserListViewModel>();

                    if (await reader.NextResultAsync())
                    {
                        int staffCodeIdx = reader.GetOrdinal("Staff_Code");
                        int nameIdx = reader.GetOrdinal("Name");
                        int emailIdx = reader.GetOrdinal("Email");
                        int imageUrlIdx = reader.GetOrdinal("Image_URL");
                        int deptNameIdx = reader.GetOrdinal("Department_Name");
                        int roleNameIdx = reader.GetOrdinal("Role_Name");
                        int statusIdx = reader.GetOrdinal("Status");

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
                                Status = !reader.IsDBNull(statusIdx) && reader.GetBoolean(statusIdx)
                            });
                        }
                    }

                    var departments = new List<DepartmentDropdownViewModel>();

                    if (await reader.NextResultAsync())
                    {
                        int deptCodeIdx = reader.GetOrdinal("Department_Code");
                        int deptNameIdx = reader.GetOrdinal("Department_Name");

                        while (await reader.ReadAsync())
                        {
                            departments.Add(new DepartmentDropdownViewModel
                            {
                                DepartmentCode = reader.IsDBNull(deptCodeIdx) ? string.Empty : reader.GetString(deptCodeIdx),
                                DepartmentName = reader.IsDBNull(deptNameIdx) ? string.Empty : reader.GetString(deptNameIdx)
                            });
                        }
                    }

                    var roles = new List<RoleDropdownViewModel>();

                    if (await reader.NextResultAsync())
                    {
                        int roleCodeIdx = reader.GetOrdinal("Role_Code");
                        int roleNameIdx = reader.GetOrdinal("Role_Name");

                        while (await reader.ReadAsync())
                        {
                            roles.Add(new RoleDropdownViewModel
                            {
                                RoleCode = reader.IsDBNull(roleCodeIdx) ? string.Empty : reader.GetString(roleCodeIdx),
                                RoleName = reader.IsDBNull(roleNameIdx) ? string.Empty : reader.GetString(roleNameIdx)
                            });
                        }
                    }

                    return (
                        overallTotalCount,
                        overallActiveCount,
                        overallInactiveCount,
                        totalCount,
                        departmentCount,
                        users,
                        departments,
                        roles
                    );
                },
                parameters);

            int errorCode = errorParam.Value != DBNull.Value
                ? Convert.ToInt32(errorParam.Value)
                : 0;

            var response = new PagedResponse<UserListViewModel>
            {
                Data = users,
                Departments = departments,
                Roles = roles,
                ErrorCode = errorCode,
                OverallTotalCount = overallTotalCount,
                OverallActiveCount = overallActiveCount,
                OverallInactiveCount = overallInactiveCount,
                TotalCount = totalCount,
                DepartmentCount = departmentCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return response;
        }
        public async Task<(int ErrorCode, string? UserName)> DeleteUserAsync(string staffCode)
        {
            var errorCodeParam = new SqlParameter("@Error_Code", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            var userNameParam = new SqlParameter("@UserName", SqlDbType.VarChar, 200)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter[] parameters =
            {
                new SqlParameter("@StaffCode", SqlDbType.VarChar, 50) { Value = staffCode },
                errorCodeParam,
                userNameParam
            };

            int errorCode = await _bdl.ExecuteNonQueryWithErrorCodeAsync("sp_UserDelete", parameters);

            string? userName = userNameParam.Value == DBNull.Value
                ? null
                : userNameParam.Value?.ToString();

            return (errorCode, userName);
        }
    }
    public class UserUpdateBL
    {
        private readonly BaseDL _bdl;

        public UserUpdateBL(BaseDL baseDL)
        {
            _bdl = baseDL;
        }
        public async Task<UserCreateViewModel?> GetUserByStaffCodeAsync(string staffCode)
        {
            if (string.IsNullOrWhiteSpace(staffCode))
                return null;

            var parameters = new[]
            {
                new SqlParameter("@Staff_Code", SqlDbType.NVarChar, 50) { Value = staffCode }
            };

            var list = await _bdl.ExecuteReaderAsync(
                "sp_GetUserByStaffCode",
                reader => new UserCreateViewModel
                {
                    StaffCode = reader["Staff_Code"]?.ToString() ?? string.Empty,
                    Name = reader["Name"]?.ToString() ?? string.Empty,
                    Email = reader["Email"]?.ToString() ?? string.Empty,
                    Gender = reader["Gender"]?.ToString() ?? string.Empty,
                    DepartmentCode = reader["Department_Code"]?.ToString() ?? string.Empty,
                    RoleCode = reader["Role_Code"]?.ToString() ?? string.Empty,
                    Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"]),
                    ImageUrl = reader["Image_URL"]?.ToString() ?? string.Empty
                },
                parameters);

            return list.FirstOrDefault();
        }
        public async Task<List<DepartmentDropdownViewModel>> GetDepartmentsAsync()
        {
            return await _bdl.ExecuteReaderAsync(
                "sp_GetDepartmentDropdown",
                reader => new DepartmentDropdownViewModel
                {
                    DepartmentCode = reader["Department_Code"]?.ToString() ?? string.Empty,
                    DepartmentName = reader["Department_Name"]?.ToString() ?? string.Empty
                });
        }

        public async Task<List<RoleDropdownViewModel>> GetRolesAsync()
        {
            return await _bdl.ExecuteReaderAsync(
                "sp_GetRoleDropdown",
                reader => new RoleDropdownViewModel
                {
                    RoleCode = reader["Role_Code"]?.ToString() ?? string.Empty,
                    RoleName = reader["Role_Name"]?.ToString() ?? string.Empty
                });
        }

        public async Task<int> UserUpdateAsync(UserCreateViewModel model)
        {
            if (model == null)
                return 3;

            var parameters = new[]
            {
                new SqlParameter("@Staff_Code", SqlDbType.NVarChar, 50) { Value = model.StaffCode },
                new SqlParameter("@Name", SqlDbType.NVarChar, 200) { Value = model.Name },
                new SqlParameter("@Email", SqlDbType.NVarChar, 200) { Value = model.Email },
                new SqlParameter("@Gender", SqlDbType.VarChar, 30) { Value = model.Gender },
                new SqlParameter("@Department_Code", SqlDbType.VarChar, 30) { Value = model.DepartmentCode },
                new SqlParameter("@Role_Code", SqlDbType.VarChar, 30) { Value = model.RoleCode },
                new SqlParameter("@Status", SqlDbType.Bit) { Value = model.Status },
                new SqlParameter("@Image_URL", SqlDbType.NVarChar, 400)
                {
                    Value = (object?)model.ImageUrl ?? DBNull.Value
                },
                new SqlParameter("@Error_Code", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                }
            };

            return await _bdl.ExecuteNonQueryWithErrorCodeAsync("sp_userUpdate", parameters);
        }
    }
}
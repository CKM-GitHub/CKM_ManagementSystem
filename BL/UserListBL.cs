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

            int overallTotalCount = 0;
            int overallActiveCount = 0;
            int overallInactiveCount = 0;
            int departmentCount = 0;
            int totalCount = 0;

            var users = new List<UserListViewModel>();
            var departments = new List<DepartmentDropdownViewModel>();
            var roles = new List<RoleDropdownViewModel>();

            var headerErrorParam = new SqlParameter("@ErrorCode", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            DataTable headerTable = await _bdl.SelectDataTableAsync(
                "sp_HeaderUserList",
                headerErrorParam);

            int headerErrorCode = headerErrorParam.Value != DBNull.Value
                ? Convert.ToInt32(headerErrorParam.Value)
                : 0;

            if (headerErrorCode == 0 &&
                headerTable.Rows.Count > 0)
            {
                DataRow row = headerTable.Rows[0];

                overallTotalCount = Convert.ToInt32(row["TotalUsers"]);
                overallActiveCount = Convert.ToInt32(row["ActiveUsers"]);
                overallInactiveCount = Convert.ToInt32(row["InactiveUsers"]);
                departmentCount = Convert.ToInt32(row["Departments"]);
            }

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

            DataSet dataSet = _bdl.SelectDataSet(
                "sp_GetUserList",
                parameters);

            int errorCode = errorParam.Value != DBNull.Value
                ? Convert.ToInt32(errorParam.Value)
                : 0;

            if (dataSet.Tables.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    users.Add(new UserListViewModel
                    {
                        StaffCode = row["Staff_Code"]?.ToString() ?? string.Empty,
                        Name = row["Name"]?.ToString() ?? string.Empty,
                        Email = row["Email"]?.ToString() ?? string.Empty,
                        ImageUrl = row["Image_URL"]?.ToString() ?? string.Empty,
                        DepartmentName = row["Department_Name"]?.ToString() ?? string.Empty,
                        RoleName = row["Role_Name"]?.ToString() ?? string.Empty,
                        Status = row["Status"] != DBNull.Value && Convert.ToBoolean(row["Status"])
                    });
                    if (totalCount == 0 && row["TotalCount"] != DBNull.Value)
                    {
                        totalCount = Convert.ToInt32(row["TotalCount"]);
                    }
                }
            }

            if (dataSet.Tables.Count > 1)
            {
                foreach (DataRow row in dataSet.Tables[1].Rows)
                {
                    departments.Add(new DepartmentDropdownViewModel
                    {
                        DepartmentCode = row["Department_Code"]?.ToString() ?? string.Empty,
                        DepartmentName = row["Department_Name"]?.ToString() ?? string.Empty
                    });
                }
            }

            if (dataSet.Tables.Count > 2)
            {
                foreach (DataRow row in dataSet.Tables[2].Rows)
                {
                    roles.Add(new RoleDropdownViewModel
                    {
                        RoleCode = row["Role_Code"]?.ToString() ?? string.Empty,
                        RoleName = row["Role_Name"]?.ToString() ?? string.Empty
                    });
                }
            }
            var response = new PagedResponse<UserListViewModel>
            {
                Data = users,
                Departments = departments,
                Roles = roles,
                ErrorCode = errorCode != 0
                    ? errorCode
                    : headerErrorCode,
                OverallTotalCount = overallTotalCount,
                OverallActiveCount = overallActiveCount,
                OverallInactiveCount = overallInactiveCount,
                DepartmentCount = departmentCount,
                TotalCount = totalCount,
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
    
    public async Task<UserCreateViewModel?> GetUserByStaffCodeAsync(string staffCode)
        {
            if (string.IsNullOrWhiteSpace(staffCode))
                return null;

            var parameters = new[]
            {
            new SqlParameter("@Staff_Code", SqlDbType.NVarChar, 50)
            {
                Value = staffCode
            }
        };

            DataTable table = await _bdl.SelectDataTableAsync(
                "sp_GetUserByStaffCode",
                parameters);

            if (table.Rows.Count == 0)
                return null;

            DataRow row = table.Rows[0];

            return new UserCreateViewModel
            {
                StaffCode = row["Staff_Code"]?.ToString() ?? string.Empty,
                Name = row["Name"]?.ToString() ?? string.Empty,
                Email = row["Email"]?.ToString() ?? string.Empty,
                Gender = row["Gender"]?.ToString() ?? string.Empty,
                DepartmentCode = row["Department_Code"]?.ToString() ?? string.Empty,
                RoleCode = row["Role_Code"]?.ToString() ?? string.Empty,
                Status = row["Status"] != DBNull.Value &&Convert.ToBoolean(row["Status"]),
                ImageUrl = row["Image_URL"]?.ToString() ?? string.Empty
            };
        }

        public async Task<List<DepartmentDropdownViewModel>> GetDepartmentsAsync()
        {
            DataTable table = await _bdl.SelectDataTableAsync(
                "sp_GetDepartmentDropdown");

            var departments = new List<DepartmentDropdownViewModel>();

            foreach (DataRow row in table.Rows)
            {
                departments.Add(new DepartmentDropdownViewModel
                {
                    DepartmentCode = row["Department_Code"]?.ToString() ?? string.Empty,
                    DepartmentName = row["Department_Name"]?.ToString() ?? string.Empty
                });
            }

            return departments;
        }

        public async Task<List<RoleDropdownViewModel>> GetRolesAsync()
        {
            DataTable table = await _bdl.SelectDataTableAsync(
                "sp_GetRoleDropdown");

            var roles = new List<RoleDropdownViewModel>();

            foreach (DataRow row in table.Rows)
            {
                roles.Add(new RoleDropdownViewModel
                {
                    RoleCode = row["Role_Code"]?.ToString() ?? string.Empty,
                    RoleName = row["Role_Name"]?.ToString() ?? string.Empty
                });
            }

            return roles;
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
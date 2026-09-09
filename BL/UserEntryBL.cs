using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CKM_ManagementSystem.BL
{
    public class UserEntryBL
    {
        private readonly BaseDL _bdl;
        private readonly PasswordService _passwordService;

        public UserEntryBL(BaseDL baseDL,PasswordService passwordService)
        {
            _bdl = baseDL;
            _passwordService = passwordService;
        }

        public async Task<int> CreateUserAsync(UserCreateViewModel model)
        {
            model.Password = _passwordService.HashPassword(model.Password);

            SqlParameter[] parameters =
            {
                new SqlParameter("@Staff_Code", SqlDbType.NVarChar, 50) { Value = model.StaffCode },
                new SqlParameter("@Name", SqlDbType.NVarChar, 200) { Value = model.Name },
                new SqlParameter("@Email", SqlDbType.NVarChar, 200) { Value = model.Email },
                new SqlParameter("@Password", SqlDbType.NVarChar, 255) { Value = model.Password },
                new SqlParameter("@Gender", SqlDbType.VarChar, 30) { Value = model.Gender },
                new SqlParameter("@Department_Code", SqlDbType.VarChar, 30) { Value = model.DepartmentCode },
                new SqlParameter("@Role_Code", SqlDbType.VarChar, 30) { Value = model.RoleCode },
                new SqlParameter("@Status", SqlDbType.Bit) { Value = model.Status },
                new SqlParameter("@Image_URL", SqlDbType.NVarChar, 400)
                {
                    Value = (object?)model.ImageUrl ?? DBNull.Value
                },
                new SqlParameter("@ErrorCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                }
            };

            return await _bdl.ExecuteNonQueryWithErrorCodeAsync("sp_CreateUser", parameters);
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
        public async Task<List<RoleDropdownViewModel>> GetUserRolesAsync()
        {
            return await _bdl.ExecuteReaderAsync(
                "sp_GetRoleDropdown",
                reader => new RoleDropdownViewModel
                {
                    RoleCode = reader["Role_Code"]?.ToString() ?? string.Empty,
                    RoleName = reader["Role_Name"]?.ToString() ?? string.Empty
                });
        }

    }
}
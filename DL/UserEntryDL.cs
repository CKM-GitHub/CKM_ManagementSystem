using CKM_ManagementSystem.DL.Interface;
using CKM_ManagementSystem.Models;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CKM_ManagementSystem.DL
{
    public class UserEntryDL : BaseDL, IUserEntryDL
    {
        public UserEntryDL(IConfiguration configuration) : base(configuration){}

        public async Task<int> CreateUserAsync(UserCreateViewModel model)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            using SqlCommand cmd = new SqlCommand("sp_CreateUser",conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Staff_Code", SqlDbType.NVarChar, 50).Value = model.StaffCode;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 200).Value = model.Name;
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = model.Email;
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 255).Value = model.Password;
            cmd.Parameters.Add("@Gender", SqlDbType.VarChar, 30).Value = model.Gender;
            cmd.Parameters.Add("@Department_Code", SqlDbType.VarChar, 30).Value = model.DepartmentCode;
            cmd.Parameters.Add("@Role_Code", SqlDbType.VarChar, 30).Value = model.RoleCode;
            cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = model.Status;
            cmd.Parameters.Add("@Image_URL", SqlDbType.NVarChar, 400).Value = model.ImageUrl ?? (object)DBNull.Value;

            SqlParameter errorCode = new SqlParameter("@ErrorCode",SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(errorCode);

            await conn.OpenAsync();

            await cmd.ExecuteNonQueryAsync();

            return Convert.ToInt32(errorCode.Value);
        }

        public async Task<IEnumerable<Department>> GetDepartmentsAsync()
        {
            var departments = new List<Department>();

            using SqlConnection conn = new SqlConnection(_connectionString);

            using SqlCommand cmd = new SqlCommand(
                "sp_GetDepartmentDropdown",
                conn);

            cmd.CommandType = CommandType.StoredProcedure;

            await conn.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                departments.Add(new Department
                {
                    DepartmentCode = reader["Department_Code"]?.ToString() ?? string.Empty,
                    DepartmentName = reader["Department_Name"]?.ToString() ?? string.Empty
                });
            }
            return departments;
        }

        public async Task<IEnumerable<UserRole>> GetUserRolesAsync()
        {
            var roles = new List<UserRole>();

            using SqlConnection conn = new SqlConnection(_connectionString);

            using SqlCommand cmd = new SqlCommand(
                "sp_GetRoleDropdown",
                conn);

            cmd.CommandType = CommandType.StoredProcedure;

            await conn.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                roles.Add(new UserRole
                {
                    RoleCode = reader["Role_Code"]?.ToString() ?? string.Empty,
                    RoleName = reader["Role_Name"]?.ToString() ?? string.Empty
                });
            }
            return roles;
        }
    }
}
using CKM_ManagementSystem.DL.Interface;
using CKM_ManagementSystem.Models;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CKM_ManagementSystem.DL
{
    public class UserUpdateDL : BaseDL, IUserUpdateDL
    {
        public UserUpdateDL(IConfiguration configuration) : base(configuration) { }
        public async Task<UserUpdateViewModel?> GetUserByStaffCodeAsync(string staffCode)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            using SqlCommand cmd = new SqlCommand("sp_GetUserByStaffCode", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Staff_Code", SqlDbType.NVarChar, 50).Value = staffCode;

            await conn.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                int staffCodeIdx = reader.GetOrdinal("Staff_Code");
                int nameIdx = reader.GetOrdinal("Name");
                int emailIdx = reader.GetOrdinal("Email");
                int genderIdx = reader.GetOrdinal("Gender");
                int deptCodeIdx = reader.GetOrdinal("Department_Code");
                int roleCodeIdx = reader.GetOrdinal("Role_Code");
                int statusIdx = reader.GetOrdinal("Status");
                int imageUrlIdx = reader.GetOrdinal("Image_URL");

                if (await reader.ReadAsync())
                {
                    return new UserUpdateViewModel
                    {
                        StaffCode = reader.IsDBNull(staffCodeIdx) ? string.Empty : reader.GetString(staffCodeIdx),
                        Name = reader.IsDBNull(nameIdx) ? string.Empty : reader.GetString(nameIdx),
                        Email = reader.IsDBNull(emailIdx) ? string.Empty : reader.GetString(emailIdx),
                        //  ImageUrl = reader.IsDBNull(imageUrlIdx) ? string.Empty : reader.GetString(imageUrlIdx),
                        Gender = reader.IsDBNull(genderIdx) ? string.Empty : reader.GetString(genderIdx),
                        DepartmentCode = reader.IsDBNull(deptCodeIdx) ? string.Empty : reader.GetString(deptCodeIdx),
                        RoleCode = reader.IsDBNull(roleCodeIdx) ? string.Empty : reader.GetString(roleCodeIdx),
                        Status = !reader.IsDBNull(statusIdx) && reader.GetBoolean(statusIdx),
                        ImageUrl = reader.IsDBNull(imageUrlIdx) ? string.Empty : reader.GetString(imageUrlIdx),
                    };
                }
            }
            return null;
        }
        public async Task<List<DepartmentDropdownViewModel>> GetDepartmentsAsync()
        {
            var departments = new List<DepartmentDropdownViewModel>();

            using SqlConnection conn =
                new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand("sp_GetDepartmentDropdown", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            await conn.OpenAsync();

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            int codeIdx = reader.GetOrdinal("Department_Code");
            int nameIdx = reader.GetOrdinal("Department_Name");

            while (await reader.ReadAsync())
            {
                departments.Add(new DepartmentDropdownViewModel
                {
                    DepartmentCode =
                        reader.IsDBNull(codeIdx)
                            ? string.Empty
                            : reader.GetString(codeIdx),

                    DepartmentName =
                        reader.IsDBNull(nameIdx)
                            ? string.Empty
                            : reader.GetString(nameIdx)
                });
            }

            return departments;
        }
        public async Task<List<RoleDropdownViewModel>> GetRolesAsync()
        {
            var roles = new List<RoleDropdownViewModel>();

            using SqlConnection conn =
                new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand("sp_GetRoleDropdown", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            await conn.OpenAsync();

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            int codeIdx = reader.GetOrdinal("Role_Code");
            int nameIdx = reader.GetOrdinal("Role_Name");

            while (await reader.ReadAsync())
            {
                roles.Add(new RoleDropdownViewModel
                {
                    RoleCode =
                        reader.IsDBNull(codeIdx)
                            ? string.Empty
                            : reader.GetString(codeIdx),

                    RoleName =
                        reader.IsDBNull(nameIdx)
                            ? string.Empty
                            : reader.GetString(nameIdx)
                });
            }

            return roles;
        }
        public async Task<int> UserUpdateAsync(UserUpdateViewModel model)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            using SqlCommand cmd = new SqlCommand("sp_userUpdate", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Staff_Code", SqlDbType.NVarChar, 50).Value = model.StaffCode;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 200).Value = model.Name;
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = model.Email;
            cmd.Parameters.Add("@Gender", SqlDbType.VarChar, 30).Value = model.Gender;
            cmd.Parameters.Add("@Department_Code", SqlDbType.VarChar, 30).Value = model.DepartmentCode;
            cmd.Parameters.Add("@Role_Code", SqlDbType.VarChar, 30).Value = model.RoleCode;
            cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = model.Status;
            cmd.Parameters.Add("@Image_URL", SqlDbType.NVarChar, 400).Value = model.ImageUrl ?? (object)DBNull.Value;
            SqlParameter errorCode = new SqlParameter("@Error_Code", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(errorCode);

            await conn.OpenAsync();

            await cmd.ExecuteNonQueryAsync();

            return Convert.ToInt32(errorCode.Value);
        }
    }
}

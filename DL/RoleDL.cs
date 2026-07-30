using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CKM_ManagementSystem.Models.ViewModels;

namespace CKM_ManagementSystem.DL
{
    public class RoleDL
    {
        private readonly string _connectionString;

        public RoleDL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> CheckDuplicateRoleCodeSPAsync(string roleCode)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_CheckDuplicateRoleCode", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RoleCode", roleCode ?? (object)DBNull.Value);

                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return result != null && Convert.ToBoolean(result);
                }
            }
        }

        public async Task<bool> SaveRoleInfoSPAsync(RoleEntryViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SaveRoleInfo", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                  
                    cmd.Parameters.AddWithValue("@RoleCode", model.RoleCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoleName", model.DisplayName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", model.Status); 
                    await conn.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }
        public async Task<bool> SaveRolePermissionSPAsync(string roleCode, int menuId, bool isAllowed)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SaveRolePermission", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RoleCode", roleCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MenuId", menuId);
                    cmd.Parameters.AddWithValue("@IsAllowed", isAllowed);

                    await conn.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        public async Task<DataTable> GetRoleByCodeSPAsync(string roleCode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetRoleByCode", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RoleCode", roleCode ?? (object)DBNull.Value);

                    await conn.OpenAsync();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
}
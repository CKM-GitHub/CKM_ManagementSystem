using System;
using System.Collections.Generic;
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

        public async Task<bool> CheckDuplicateRoleCodeAsync(string roleCode)
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

        public async Task SaveRoleWithPermissionsAsync(RoleEntryViewModel model)
        {
            bool isSuccess = await SaveRoleInfoSPAsync(model);

            if (!isSuccess)
            {
                throw new Exception("Role Information ကို သိမ်းဆည်းရာတွင် မအောင်မြင်ပါ။");
            }

            if (model.MenuPermissions != null && model.MenuPermissions.Count > 0)
            {
                foreach (var permission in model.MenuPermissions)
                {
                    await SaveRolePermissionSPAsync(
                        model.RoleCode,
                        permission.MenuId,
                        permission.CanRead,
                        permission.CanWrite,
                        permission.CanDelete
                    );
                }
            }
        }

        public async Task<bool> SaveRoleInfoSPAsync(RoleEntryViewModel model)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SaveRoleInfo", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@RoleCode", SqlDbType.VarChar, 30).Value = (object)model.RoleCode ?? DBNull.Value;
                        cmd.Parameters.Add("@RoleName", SqlDbType.NVarChar, 100).Value = (object)model.DisplayName ?? DBNull.Value;
                        cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 255).Value = (object)model.Description ?? DBNull.Value;
                        cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = model.Status;

                        await conn.OpenAsync();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"SQL Save Error: {ex.Message}");
            }
        }

        public async Task<bool> SaveRolePermissionSPAsync(string roleCode, int menuId, bool canRead, bool canWrite, bool canDelete)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SaveRolePermission", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@RoleCode", SqlDbType.VarChar, 30).Value = (object)roleCode ?? DBNull.Value;
                    cmd.Parameters.Add("@MenuId", SqlDbType.Int).Value = menuId;
                    cmd.Parameters.Add("@CanRead", SqlDbType.Bit).Value = canRead;
                    cmd.Parameters.Add("@CanWrite", SqlDbType.Bit).Value = canWrite;
                    cmd.Parameters.Add("@CanDelete", SqlDbType.Bit).Value = canDelete;

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

        public async Task<RoleListPagedViewModel> GetRoleListAsync(string searchKeyword, int? status, int page, int pageSize)
        {
            var result = new RoleListPagedViewModel
            {
                PageNumber = page,
                PageSize = pageSize,
                SearchKeyword = searchKeyword,
                Status = status,
                Roles = new List<RoleEntryViewModel>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetRoleList", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    string searchVal = string.IsNullOrWhiteSpace(searchKeyword) ? null : searchKeyword.Trim();
                    cmd.Parameters.AddWithValue("@SearchKeyword", (object)searchVal ?? DBNull.Value);

                    if (status.HasValue && status.Value != -1)
                    {
                        cmd.Parameters.AddWithValue("@Status", status.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Status", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@PageNumber", page);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    SqlParameter totalRecordsParam = new SqlParameter("@TotalRecords", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(totalRecordsParam);

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string roleCode = reader.HasColumn("RoleCode") ? reader["RoleCode"]?.ToString() :
                                               reader.HasColumn("Role_Code") ? reader["Role_Code"]?.ToString() : "";

                            string displayName = reader.HasColumn("DisplayName") ? reader["DisplayName"]?.ToString() :
                                                 reader.HasColumn("Role_Name") ? reader["Role_Name"]?.ToString() : "";

                            string description = reader.HasColumn("Description") ? reader["Description"]?.ToString() : "";

                            bool isStatus = reader.HasColumn("Status") && reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"]);

                            result.Roles.Add(new RoleEntryViewModel
                            {
                                RoleCode = roleCode,
                                DisplayName = displayName,
                                Description = description,
                                Status = isStatus
                            });
                        }
                    } 

                    if (totalRecordsParam.Value != DBNull.Value && totalRecordsParam.Value != null)
                    {
                        result.TotalRecords = Convert.ToInt32(totalRecordsParam.Value);
                    }
                }
            }

            return result;
        }

        public async Task<bool> DeleteRoleAsync(string roleCode)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteRole", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RoleCode", roleCode ?? (object)DBNull.Value);

                    await conn.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }
    }

    public static class SqlDataReaderExtensions
    {
        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Roles;
using CKM_ManagementSystem.Models.ViewModels;

namespace CKM_ManagementSystem.BL
{
    public class RoleBL
    {
        private readonly string _connectionString;

        public RoleBL(IConfiguration configuration)
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

        public async Task<bool> SaveRolePermissionSPAsync(string roleCode, int menuId, bool canRead, bool canWrite, bool canDelete)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SaveRolePermission", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RoleCode", roleCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MenuId", menuId);
                    cmd.Parameters.AddWithValue("@CanRead", canRead);
                    cmd.Parameters.AddWithValue("@CanWrite", canWrite);
                    cmd.Parameters.AddWithValue("@CanDelete", canDelete);

                    await conn.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        
        public async Task<bool> SaveRoleWithPermissionsSPAsync(RoleEntryViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        
                        using (SqlCommand cmdRole = new SqlCommand("sp_SaveRoleInfo", conn, transaction))
                        {
                            cmdRole.CommandType = CommandType.StoredProcedure;
                            cmdRole.Parameters.AddWithValue("@RoleCode", model.RoleCode ?? (object)DBNull.Value);
                            cmdRole.Parameters.AddWithValue("@RoleName", model.DisplayName ?? (object)DBNull.Value);
                            cmdRole.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
                            cmdRole.Parameters.AddWithValue("@Status", model.Status);

                            await cmdRole.ExecuteNonQueryAsync();
                        }

                        
                        if (model.MenuPermissions != null && model.MenuPermissions.Any())
                        {
                            foreach (var perm in model.MenuPermissions)
                            {
                                using (SqlCommand cmdPerm = new SqlCommand("sp_SaveRolePermission", conn, transaction))
                                {
                                    cmdPerm.CommandType = CommandType.StoredProcedure;
                                    cmdPerm.Parameters.AddWithValue("@RoleCode", model.RoleCode ?? (object)DBNull.Value);
                                    cmdPerm.Parameters.AddWithValue("@MenuId", perm.MenuId);
                                    cmdPerm.Parameters.AddWithValue("@CanRead", perm.CanRead);
                                    cmdPerm.Parameters.AddWithValue("@CanWrite", perm.CanWrite);
                                    cmdPerm.Parameters.AddWithValue("@CanDelete", perm.CanDelete);

                                    await cmdPerm.ExecuteNonQueryAsync();
                                }
                            }
                        }

                        
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception)
                    {
                        
                        await transaction.RollbackAsync();
                        throw;
                    }
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

        public async Task<RoleListPagedViewModel> GetRoleListAsync(string search, int? status, int page, int pageSize)
        {
            int safePage = page < 1 ? 1 : page;
            int safePageSize = pageSize < 1 ? 10 : pageSize;

            var model = new RoleListPagedViewModel
            {
                PageNumber = safePage,
                PageSize = safePageSize,
                SearchKeyword = search,
                Status = status
            };

            var roles = new List<RoleEntryViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetRoleList", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@SearchKeyword", string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : search);
                    cmd.Parameters.AddWithValue("@Status", status.HasValue ? status.Value : (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@PageNumber", safePage);
                    cmd.Parameters.AddWithValue("@PageSize", safePageSize);

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
                            roles.Add(new RoleEntryViewModel
                            {
                                RoleCode = reader["RoleCode"] != DBNull.Value ? reader["RoleCode"].ToString() : "",
                                DisplayName = reader["DisplayName"] != DBNull.Value ? reader["DisplayName"].ToString() : "",
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                                Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"])
                            });
                        }
                    }

                    if (totalRecordsParam.Value != DBNull.Value)
                    {
                        model.TotalRecords = Convert.ToInt32(totalRecordsParam.Value);
                    }
                }
            }

            model.Roles = roles;
            return model;
        }

        public async Task<RoleEntryViewModel> GetRoleByCodeAsync(string roleCode)
        {
            DataTable dt = await GetRoleByCodeSPAsync(roleCode);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new RoleEntryViewModel
                {
                    RoleCode = row.Table.Columns.Contains("RoleCode") ? row["RoleCode"].ToString() : row["Role_Code"].ToString(),
                    DisplayName = row.Table.Columns.Contains("DisplayName") ? row["DisplayName"].ToString() : row["Role_Name"].ToString(),
                    Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                    Status = Convert.ToBoolean(row["Status"])
                };
            }
            return null;
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
}
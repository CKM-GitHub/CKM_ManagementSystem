using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CKM_ManagementSystem.Models.ViewModels.Roles;

namespace CKM_ManagementSystem.BL
{
    public class RoleBL
    {
        private readonly string _connectionString;

        public RoleBL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region Service Wrapper Methods

        public async Task<List<MenuPermissionViewModel>> GetMenuPermissionsAsync(string roleCode = null)
        {
            return await GetMenuPermissionsSPAsync(roleCode);
        }

        public async Task<bool> CheckDuplicateRoleCodeAsync(string roleCode)
        {
            return await CheckDuplicateRoleCodeSPAsync(roleCode);
        }

        public async Task SaveRoleWithPermissionsAsync(RoleEntryViewModel model)
        {
            await SaveRoleInfoSPAsync(model);

            if (model.MenuPermissions != null && model.MenuPermissions.Count > 0)
            {
                foreach (var perm in model.MenuPermissions)
                {
                    await SaveRolePermissionSPAsync(
                        model.RoleCode,
                        perm.MenuId,
                        perm.CanRead,
                        perm.CanWrite,
                        perm.CanDelete
                    );
                }
            }
        }

        #endregion

        #region Stored Procedure Implementations

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

        public async Task<bool> CheckRoleExistsSPAsync(string roleCode)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT COUNT(1) FROM UserRoles WHERE RoleCode = @RoleCode";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleCode", roleCode ?? (object)DBNull.Value);
                    int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }

        public async Task<List<MenuPermissionViewModel>> GetMenuPermissionsSPAsync(string roleCode = null)
        {
            var list = new List<MenuPermissionViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query;

                if (string.IsNullOrEmpty(roleCode))
                {
                    query = @"
                        SELECT 
                            m.MenuID AS MenuId,
                            m.MenuName,
                            m.ParentMenuId AS ParentId,
                            CAST(0 AS BIT) AS CanRead,
                            CAST(0 AS BIT) AS CanWrite,
                            CAST(0 AS BIT) AS CanDelete
                        FROM Menus m
                        ORDER BY 
                            COALESCE(m.ParentMenuId, m.MenuID),
                            CASE WHEN m.ParentMenuId IS NULL THEN 0 ELSE 1 END,
                            m.DisplayOrder,
                            m.MenuID";
                }
                else
                {
                    query = @"
                        SELECT 
                            m.MenuID AS MenuId,
                            m.MenuName,
                            m.ParentMenuId AS ParentId,
                            ISNULL(p.CanRead, 0) AS CanRead,
                            ISNULL(p.CanWrite, 0) AS CanWrite,
                            ISNULL(p.CanDelete, 0) AS CanDelete
                        FROM Menus m
                        LEFT JOIN UserRolePermissions p ON m.MenuID = p.MenuID 
                            AND p.RoleCode = @RoleCode
                        ORDER BY 
                            COALESCE(m.ParentMenuId, m.MenuID),
                            CASE WHEN m.ParentMenuId IS NULL THEN 0 ELSE 1 END,
                            m.DisplayOrder,
                            m.MenuID";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(roleCode))
                    {
                        cmd.Parameters.AddWithValue("@RoleCode", roleCode);
                    }

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new MenuPermissionViewModel
                            {
                                MenuId = Convert.ToInt32(reader["MenuId"]),
                                MenuName = reader["MenuName"].ToString(),
                                ParentId = reader["ParentId"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["ParentId"]),
                                CanRead = Convert.ToBoolean(reader["CanRead"]),
                                CanWrite = Convert.ToBoolean(reader["CanWrite"]),
                                CanDelete = Convert.ToBoolean(reader["CanDelete"])
                            });
                        }
                    }
                }
            }
            return list;
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

        public async Task<RoleEntryViewModel> GetRoleByCodeSPAsync(string roleCode)
        {
            RoleEntryViewModel role = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetRoleByCode", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RoleCode", roleCode ?? (object)DBNull.Value);

                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            role = new RoleEntryViewModel
                            {
                                RoleCode = reader["RoleCode"].ToString(),
                                DisplayName = reader["DisplayName"] != DBNull.Value ? reader["DisplayName"].ToString() : string.Empty,
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : string.Empty,
                                Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"])
                            };
                        }
                    }
                }
            }
            return role;
        }

        #endregion
    }
}
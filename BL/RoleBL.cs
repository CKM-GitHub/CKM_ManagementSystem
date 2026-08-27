using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.Models.ViewModels.Roles;
using Microsoft.Data.SqlClient;

namespace CKM_ManagementSystem.BL
{
    public class RoleBL
    {
        private readonly BaseDL bdl;
        private readonly string _connectionString;

        public RoleBL(BaseDL baseDL, string connectionString = "")
        {
            bdl = baseDL;
            _connectionString = connectionString;
        }

        #region Synchronous / Stored Procedure Methods (Legacy)

        public string Role_Insert(Roles role, List<RolePermission> permissions)
        {
            DataTable dtPermissions = ConvertPermissionsToDataTable(permissions);

            SqlParameter paramPermissions = new SqlParameter("@Permissions", SqlDbType.Structured)
            {
                TypeName = "dbo.RolePermissionType",
                Value = dtPermissions
            };

            SqlParameter[] sqlprms =
            {
                new SqlParameter("@Role_Code", (object)role.RoleCode ?? string.Empty),
                new SqlParameter("@Role_Name", (object)role.RoleName ?? string.Empty),
                new SqlParameter("@Description", (object)role.Description ?? DBNull.Value),
                new SqlParameter("@Status", role.Status),
                paramPermissions
            };

            return bdl.InsertUpdateDeleteData("sp_SaveRoleInfo", sqlprms);
        }

        public string Role_Update(Roles role, List<RolePermission> permissions)
        {
            DataTable dtPermissions = ConvertPermissionsToDataTable(permissions);

            SqlParameter paramPermissions = new SqlParameter("@Permissions", SqlDbType.Structured)
            {
                TypeName = "dbo.RolePermissionType",
                Value = dtPermissions
            };

            SqlParameter[] sqlprms =
            {
                new SqlParameter("@Role_Code", (object)role.RoleCode ?? string.Empty),
                new SqlParameter("@Role_Name", (object)role.RoleName ?? string.Empty),
                new SqlParameter("@Description", (object)role.Description ?? DBNull.Value),
                new SqlParameter("@Status", role.Status),
                paramPermissions
            };

            return bdl.InsertUpdateDeleteData("sp_SaveRoleInfo", sqlprms);
        }

        public DataTable GetRoleList()
        {
            return bdl.SelectData("sp_GetRoleList");
        }

        public DataTable GetRoleByCode(string roleCode)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@RoleCode", (object)roleCode ?? string.Empty)
            };

            return bdl.SelectData("sp_GetRoleByCode", sqlprms);
        }

        public DataTable GetRolePermissionsByCode(string roleCode)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@RoleCode", (object)roleCode ?? string.Empty)
            };

            DataTable dt = bdl.SelectData("sp_GetRolePermission", sqlprms);
            StandardizeMenuColumns(dt);
            return dt;
        }

        public DataTable GetAllMenus()
        {
            DataTable dt = bdl.SelectData("sp_GetMenuList");
            StandardizeMenuColumns(dt);
            return dt;
        }

        public bool IsRoleCodeDuplicate(string roleCode)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@RoleCode", (object)roleCode ?? string.Empty)
            };

            object result = bdl.ExecuteScalar("sp_CheckDuplicateRoleCode", sqlprms);

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result) > 0;
            }

            return false;
        }

        #endregion

        #region Async / Direct SQL Methods

        public async Task<RoleListPagedViewModel> GetRoleListPagedAsync(int pageNumber, int pageSize, string searchKeyword, int? status)
        {
            return await GetRoleListPagedSPAsync(pageNumber, pageSize, searchKeyword, status);
        }

        public async Task<List<MenuPermissionViewModel>> GetMenuPermissionsAsync(string? roleCode = null)
        {
            return await GetMenuPermissionsSPAsync(roleCode);
        }

        public async Task<RoleEntryViewModel?> GetRoleByCodeAsync(string roleCode)
        {
            return await GetRoleByCodeSPAsync(roleCode);
        }

        public async Task<bool> IsRoleActiveAsync(string roleCode)
        {
            var role = await GetRoleByCodeSPAsync(roleCode);
            return role != null && role.Status;
        }

        public async Task<(bool Success, string Message)> DeleteRoleAsync(string roleCode)
        {
            return await DeleteRoleSPAsync(roleCode);
        }

        public async Task<RoleListPagedViewModel> GetRoleListPagedSPAsync(int pageNumber, int pageSize, string searchKeyword, int? status)
        {
            var result = new RoleListPagedViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchKeyword = searchKeyword,
                Status = status
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"
                    SELECT COUNT(1) 
                    FROM UserRoles 
                    WHERE (@SearchKeyword IS NULL OR Role_Code LIKE '%' + @SearchKeyword + '%' OR Role_Name LIKE '%' + @SearchKeyword + '%')
                      AND (@Status IS NULL OR Status = @Status);

                    SELECT 
                        Role_Code AS RoleCode, 
                        Role_Name AS DisplayName, 
                        Description, 
                        Status
                    FROM UserRoles
                    WHERE (@SearchKeyword IS NULL OR Role_Code LIKE '%' + @SearchKeyword + '%' OR Role_Name LIKE '%' + @SearchKeyword + '%')
                      AND (@Status IS NULL OR Status = @Status)
                    ORDER BY ISNULL(Updated_Date, Created_Date) DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SearchKeyword", string.IsNullOrWhiteSpace(searchKeyword) ? (object)DBNull.Value : searchKeyword);
                    cmd.Parameters.AddWithValue("@Status", status.HasValue ? (object)status.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            result.TotalRecords = Convert.ToInt32(reader[0]);
                        }

                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Roles.Add(new RoleEntryViewModel
                                {
                                    RoleCode = reader["RoleCode"].ToString() ?? string.Empty,
                                    DisplayName = reader["DisplayName"] != DBNull.Value ? reader["DisplayName"].ToString() ?? string.Empty : string.Empty,
                                    Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() ?? string.Empty : string.Empty,
                                    Status = ParseStatus(reader["Status"])
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }

        public async Task<bool> CheckDuplicateRoleCodeSPAsync(string roleCode)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT COUNT(1) FROM UserRoles WHERE Role_Code = @RoleCode";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleCode", roleCode ?? (object)DBNull.Value);
                    int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }

        public async Task<bool> CheckRoleExistsSPAsync(string roleCode)
        {
            return await CheckDuplicateRoleCodeSPAsync(roleCode);
        }

        public async Task<List<MenuPermissionViewModel>> GetMenuPermissionsSPAsync(string? roleCode = null)
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
                            AND p.Role_Code = @RoleCode
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
                                MenuName = reader["MenuName"].ToString() ?? string.Empty,
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

        public async Task<RoleEntryViewModel?> GetRoleByCodeSPAsync(string roleCode)
        {
            RoleEntryViewModel? role = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"
                    SELECT 
                        Role_Code AS RoleCode, 
                        Role_Name AS DisplayName, 
                        Description, 
                        Status
                    FROM UserRoles 
                    WHERE Role_Code = @RoleCode";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleCode", roleCode ?? (object)DBNull.Value);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            role = new RoleEntryViewModel
                            {
                                RoleCode = reader["RoleCode"].ToString() ?? string.Empty,
                                DisplayName = reader["DisplayName"] != DBNull.Value ? reader["DisplayName"].ToString() ?? string.Empty : string.Empty,
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() ?? string.Empty : string.Empty,
                                Status = ParseStatus(reader["Status"])
                            };
                        }
                    }
                }
            }

            return role;
        }

        public async Task<(bool Success, string Message)> DeleteRoleSPAsync(string roleCode)
        {
            var role = await GetRoleByCodeSPAsync(roleCode);
            if (role == null)
            {
                return (false, "Role not found.");
            }

            if (role.Status)
            {
                return (false, "Cannot delete an active role.");
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string deletePermsQuery = "DELETE FROM UserRolePermissions WHERE Role_Code = @RoleCode";
                        using (SqlCommand cmdPerm = new SqlCommand(deletePermsQuery, conn, transaction))
                        {
                            cmdPerm.Parameters.AddWithValue("@RoleCode", roleCode ?? (object)DBNull.Value);
                            await cmdPerm.ExecuteNonQueryAsync();
                        }

                        string deleteRoleQuery = "DELETE FROM UserRoles WHERE Role_Code = @RoleCode";
                        using (SqlCommand cmdRole = new SqlCommand(deleteRoleQuery, conn, transaction))
                        {
                            cmdRole.Parameters.AddWithValue("@RoleCode", roleCode ?? (object)DBNull.Value);
                            await cmdRole.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                        return (true, "Role deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return (false, "An error occurred while deleting the role: " + ex.Message);
                    }
                }
            }
        }

        #endregion

        #region Private Helper Methods

        private static void StandardizeMenuColumns(DataTable dt)
        {
            if (dt == null) return;

            if (dt.Columns.Contains("ParentMenuId") && !dt.Columns.Contains("ParentId"))
            {
                dt.Columns["ParentMenuId"].ColumnName = "ParentId";
            }

            if (dt.Columns.Contains("MenuID") && !dt.Columns.Contains("MenuId"))
            {
                dt.Columns["MenuID"].ColumnName = "MenuId";
            }
        }

        private bool ParseStatus(object statusObj)
        {
            if (statusObj == null || statusObj == DBNull.Value)
                return false;

            if (statusObj is bool b)
                return b;

            if (int.TryParse(statusObj.ToString(), out int val))
            {
                return val == 1;
            }

            string str = statusObj.ToString().Trim();
            return str.Equals("true", StringComparison.OrdinalIgnoreCase) || str == "1";
        }

        private static DataTable ConvertPermissionsToDataTable(List<RolePermission> permissions)
        {
            DataTable dt = GetEmptyPermissionsTable();

            if (permissions != null && permissions.Count > 0)
            {
                foreach (var item in permissions)
                {
                    dt.Rows.Add(
                        item.MenuId,
                        item.CanRead,
                        item.CanWrite,
                        item.CanDelete
                    );
                }
            }

            return dt;
        }

        private static DataTable GetEmptyPermissionsTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MenuId", typeof(int));
            dt.Columns.Add("CanRead", typeof(bool));
            dt.Columns.Add("CanWrite", typeof(bool));
            dt.Columns.Add("CanDelete", typeof(bool));
            return dt;
        }

        #endregion
    }
}
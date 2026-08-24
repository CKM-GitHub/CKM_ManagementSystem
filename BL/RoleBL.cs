using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CKM_ManagementSystem.Models.ViewModels;
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

        #region Helper Methods

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

        #endregion

        #region Service Wrapper Methods

        public async Task<RoleListPagedViewModel> GetRoleListPagedAsync(int pageNumber, int pageSize, string searchKeyword, int? status)
        {
            return await GetRoleListPagedSPAsync(pageNumber, pageSize, searchKeyword, status);
        }

        public async Task<List<MenuPermissionViewModel>> GetMenuPermissionsAsync(string? roleCode = null)
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

        #endregion

        #region Query Implementations

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

        public async Task<bool> SaveRoleInfoSPAsync(RoleEntryViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"
                    IF EXISTS (SELECT 1 FROM UserRoles WHERE Role_Code = @RoleCode)
                    BEGIN
                        UPDATE UserRoles 
                        SET Role_Name = @RoleName,
                            Description = @Description,
                            Status = @Status,
                            Updated_Date = GETDATE()
                        WHERE Role_Code = @RoleCode
                    END
                    ELSE
                    BEGIN
                        INSERT INTO UserRoles (Role_Code, Role_Name, Description, Status, Created_Date)
                        VALUES (@RoleCode, @RoleName, @Description, @Status, GETDATE())
                    END";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleCode", model.RoleCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoleName", model.DisplayName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", model.Status ? 1 : 0);

                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        public async Task<bool> SaveRolePermissionSPAsync(string roleCode, int menuId, bool canRead, bool canWrite, bool canDelete)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"
                    IF EXISTS (SELECT 1 FROM UserRolePermissions WHERE Role_Code = @RoleCode AND MenuID = @MenuId)
                    BEGIN
                        UPDATE UserRolePermissions 
                        SET CanRead = @CanRead, CanWrite = @CanWrite, CanDelete = @CanDelete
                        WHERE Role_Code = @RoleCode AND MenuID = @MenuId
                    END
                    ELSE
                    BEGIN
                        INSERT INTO UserRolePermissions (Role_Code, MenuID, CanRead, CanWrite, CanDelete)
                        VALUES (@RoleCode, @MenuId, @CanRead, @CanWrite, @CanDelete)
                    END";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleCode", roleCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MenuId", menuId);
                    cmd.Parameters.AddWithValue("@CanRead", canRead);
                    cmd.Parameters.AddWithValue("@CanWrite", canWrite);
                    cmd.Parameters.AddWithValue("@CanDelete", canDelete);

                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
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
    }
}
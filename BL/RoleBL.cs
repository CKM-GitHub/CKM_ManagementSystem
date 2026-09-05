using System;
using System.Collections.Generic;
using System.Data;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.Models.ViewModels.Roles;
using Microsoft.Data.SqlClient;

namespace CKM_ManagementSystem.BL
{
    public class RoleBL
    {
        private readonly BaseDL _bdl;

        public RoleBL(BaseDL bdl)
        {
            _bdl = bdl;
        }

        #region Role Save / Update / Get Standard SPs

        public string Role_Insert(Roles role, List<RolePermission> permissions)
        {
            DataTable dtPermissions = ConvertPermissionsToDataTable(permissions);
            return SaveRoleInfo(role, dtPermissions);
        }

        public string Role_Update(Roles role, List<RolePermission> permissions)
        {
            DataTable dtPermissions = ConvertPermissionsToDataTable(permissions);
            return SaveRoleInfo(role, dtPermissions);
        }

        private string SaveRoleInfo(Roles role, DataTable dtPermissions)
        {
            SqlParameter paramPermissions = new SqlParameter("@Permissions", SqlDbType.Structured)
            {
                TypeName = "dbo.RolePermissionType",
                Value = dtPermissions
            };

            SqlParameter[] sqlprms =
            {
                new SqlParameter("@Role_Code", (object?)role.RoleCode ?? string.Empty),
                new SqlParameter("@Role_Name", (object?)role.RoleName ?? string.Empty),
                new SqlParameter("@Description", (object?)role.Description ?? DBNull.Value),
                new SqlParameter("@Status", role.Status),
                paramPermissions
            };

            return _bdl.InsertUpdateDeleteData("sp_SaveRoleInfo", sqlprms);
        }

        public DataTable GetRoleList()
        {
            return _bdl.SelectData("sp_GetRoleList");
        }

        public DataTable GetRoleByCode(string roleCode)
        {
            SqlParameter[] sqlprms = { new SqlParameter("@RoleCode", (object?)roleCode ?? string.Empty) };
            return _bdl.SelectData("sp_GetRoleByCode", sqlprms);
        }

        public DataTable GetRolePermissionsByCode(string roleCode)
        {
            SqlParameter[] sqlprms = { new SqlParameter("@RoleCode", (object?)roleCode ?? string.Empty) };
            DataTable dt = _bdl.SelectData("sp_GetRolePermission", sqlprms);
            StandardizeMenuColumns(dt);
            return dt;
        }

        public DataTable GetAllMenus()
        {
            DataTable dt = _bdl.SelectData("sp_GetMenuList");
            StandardizeMenuColumns(dt);
            return dt;
        }

        public bool IsRoleCodeDuplicate(string roleCode)
        {
            SqlParameter[] sqlprms = { new SqlParameter("@RoleCode", (object?)roleCode ?? string.Empty) };
            var scalarResult = _bdl.ExecuteScalar("sp_CheckDuplicateRoleCode", sqlprms);
            int result = scalarResult != null ? Convert.ToInt32(scalarResult) : 0;
            return result > 0;
        }

        #endregion

        #region Paging, Menu Permissions & Delete via Stored Procedures

        public RoleListPagedViewModel GetRoleListPaged(int pageNumber, int pageSize, string searchKeyword, int? status)
        {
            SqlParameter[] prmsCount = {
                new SqlParameter("@SearchKeyword", string.IsNullOrWhiteSpace(searchKeyword) ? (object)DBNull.Value : searchKeyword),
                new SqlParameter("@Status", status.HasValue ? (object)status.Value : DBNull.Value)
            };

            DataTable dtCount = _bdl.SelectData("sp_GetRoleList", prmsCount);
            int totalRecords = dtCount != null ? dtCount.Rows.Count : 0;

            SqlParameter[] prmsData = {
                new SqlParameter("@SearchKeyword", string.IsNullOrWhiteSpace(searchKeyword) ? (object)DBNull.Value : searchKeyword),
                new SqlParameter("@Status", status.HasValue ? (object)status.Value : DBNull.Value),
                new SqlParameter("@Offset", (pageNumber - 1) * pageSize),
                new SqlParameter("@PageSize", pageSize)
            };

            DataTable dtData = _bdl.SelectData("sp_GetRoleListPaged", prmsData);

            var roles = new List<RoleEntryViewModel>();
            if (dtData != null)
            {
                foreach (DataRow row in dtData.Rows)
                {
                    string code = dtData.Columns.Contains("RoleCode") ? row["RoleCode"]?.ToString() ?? ""
                                : dtData.Columns.Contains("Role_Code") ? row["Role_Code"]?.ToString() ?? "" : "";

                    string name = dtData.Columns.Contains("DisplayName") ? row["DisplayName"]?.ToString() ?? ""
                                : dtData.Columns.Contains("Role_Name") ? row["Role_Name"]?.ToString() ?? "" : "";

                    roles.Add(new RoleEntryViewModel
                    {
                        RoleCode = code,
                        DisplayName = name,
                        Description = dtData.Columns.Contains("Description") && row["Description"] != DBNull.Value ? row["Description"]?.ToString() ?? "" : "",
                        Status = ParseStatus(row["Status"])
                    });
                }
            }

            return new RoleListPagedViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchKeyword = searchKeyword,
                Status = status,
                TotalRecords = totalRecords,
                Roles = roles
            };
        }

        public List<MenuPermissionViewModel> GetMenuPermissions(string? roleCode = null)
        {
            SqlParameter[] prms = {
                new SqlParameter("@RoleCode", string.IsNullOrEmpty(roleCode) ? (object)DBNull.Value : roleCode)
            };

            DataTable dt = _bdl.SelectData("sp_GetMenuPermissions", prms);
            var list = new List<MenuPermissionViewModel>();

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    int menuId = row.Table.Columns.Contains("MenuId") ? Convert.ToInt32(row["MenuId"]) : Convert.ToInt32(row["MenuID"]);

                    int? parentId = null;
                    if (row.Table.Columns.Contains("ParentId") && row["ParentId"] != DBNull.Value)
                    {
                        parentId = Convert.ToInt32(row["ParentId"]);
                    }
                    else if (row.Table.Columns.Contains("ParentMenuId") && row["ParentMenuId"] != DBNull.Value)
                    {
                        parentId = Convert.ToInt32(row["ParentMenuId"]);
                    }

                    list.Add(new MenuPermissionViewModel
                    {
                        MenuId = menuId,
                        MenuName = row["MenuName"]?.ToString() ?? string.Empty,
                        ParentId = parentId,
                        CanRead = row["CanRead"] != DBNull.Value && Convert.ToBoolean(row["CanRead"]),
                        CanWrite = row["CanWrite"] != DBNull.Value && Convert.ToBoolean(row["CanWrite"]),
                        CanDelete = row["CanDelete"] != DBNull.Value && Convert.ToBoolean(row["CanDelete"])
                    });
                }
            }

            return list;
        }

        public RoleEntryViewModel? GetRoleByCodeViewModel(string roleCode)
        {
            DataTable dt = GetRoleByCode(roleCode);
            if (dt == null || dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];

            string code = dt.Columns.Contains("RoleCode") ? row["RoleCode"]?.ToString() ?? ""
                        : dt.Columns.Contains("Role_Code") ? row["Role_Code"]?.ToString() ?? "" : "";

            string name = dt.Columns.Contains("DisplayName") ? row["DisplayName"]?.ToString() ?? ""
                        : dt.Columns.Contains("Role_Name") ? row["Role_Name"]?.ToString() ?? "" : "";

            string desc = dt.Columns.Contains("Description") && row["Description"] != DBNull.Value
                        ? row["Description"]?.ToString() ?? "" : "";

            return new RoleEntryViewModel
            {
                RoleCode = code,
                DisplayName = name,
                Description = desc,
                Status = dt.Columns.Contains("Status") ? ParseStatus(row["Status"]) : false
            };
        }

        public (bool Success, string Message) DeleteRole(string roleCode)
        {
            var role = GetRoleByCodeViewModel(roleCode);
            if (role == null)
            {
                return (false, "Role not found.");
            }

            if (role.Status)
            {
                return (false, "Cannot delete an active role.");
            }

            SqlParameter[] prms = { new SqlParameter("@RoleCode", (object?)roleCode ?? DBNull.Value) };
            string result = _bdl.InsertUpdateDeleteData("sp_DeleteRole", prms);

            if (string.IsNullOrEmpty(result) || result.Equals("true", StringComparison.OrdinalIgnoreCase) || result == "1")
            {
                return (true, "Role deleted successfully.");
            }

            return (false, "An error occurred while deleting the role: " + result);
        }

        #endregion

        #region Private Helpers

        private static bool ParseStatus(object? statusObj)
        {
            if (statusObj == null || statusObj == DBNull.Value) return false;
            if (statusObj is bool b) return b;
            if (int.TryParse(statusObj.ToString(), out int val)) return val == 1;
            string str = statusObj.ToString()!.Trim();
            return str.Equals("true", StringComparison.OrdinalIgnoreCase) || str == "1";
        }

        private static void StandardizeMenuColumns(DataTable dt)
        {
            if (dt == null) return;

            string[] possibleParentCols = { "ParentMenuId", "Parent_Menu_Id", "ParentMenuID", "Parent_Menu_ID" };
            foreach (var colName in possibleParentCols)
            {
                if (dt.Columns.Contains(colName) && colName != "ParentId")
                {
                    dt.Columns[colName].ColumnName = "ParentId";
                    break;
                }
            }

            string[] possibleMenuCols = { "MenuID", "Menu_Id", "Menu_ID" };
            foreach (var colName in possibleMenuCols)
            {
                if (dt.Columns.Contains(colName) && colName != "MenuId")
                {
                    dt.Columns[colName].ColumnName = "MenuId";
                    break;
                }
            }
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
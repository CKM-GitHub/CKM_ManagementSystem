using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.Entities;
<<<<<<< HEAD
=======
using CKM_ManagementSystem.Models.ViewModels.Roles;
>>>>>>> 5-rolelist
using Microsoft.Data.SqlClient;

namespace CKM_ManagementSystem.BL
{
    public class RoleBL
    {
        private readonly BaseDL _bdl;

<<<<<<< HEAD
        public RoleBL(BaseDL baseDL)
        {
            _bdl = baseDL ?? throw new ArgumentNullException(nameof(baseDL));
        }

        public string Role_Insert(Roles role, List<RolePermission> permissions)
        {
            return SaveRoleInfo(role, permissions);
=======
        public RoleBL(BaseDL bdl)
        {
            _bdl = bdl ?? throw new ArgumentNullException(nameof(bdl));
        }

        #region Role Save / Update / Get Standard SPs

        public string Role_Insert(Roles role, List<RolePermission> permissions)
        {
            DataTable dtPermissions = ConvertPermissionsToDataTable(permissions);
            return SaveRoleInfo(role, dtPermissions);
>>>>>>> 5-rolelist
        }

        public string Role_Update(Roles role, List<RolePermission> permissions)
        {
<<<<<<< HEAD
            return SaveRoleInfo(role, permissions);
        }

        private string SaveRoleInfo(Roles role, List<RolePermission> permissions)
        {
            DataTable dtPermissions = ConvertPermissionsToDataTable(permissions);

=======
            DataTable dtPermissions = ConvertPermissionsToDataTable(permissions);
            return SaveRoleInfo(role, dtPermissions);
        }

        private string SaveRoleInfo(Roles role, DataTable dtPermissions)
        {
>>>>>>> 5-rolelist
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

<<<<<<< HEAD
=======
        public DataTable GetRoleList()
        {
            return _bdl.SelectData("sp_GetRoleList");
        }

>>>>>>> 5-rolelist
        public bool IsRoleCodeDuplicate(string roleCode)
        {
            SqlParameter[] sqlprms =
            {
<<<<<<< HEAD
                new SqlParameter("@RoleCode", (object?)roleCode ?? string.Empty)
            };

            object result = _bdl.ExecuteScalar("sp_CheckDuplicateRoleCode", sqlprms);

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToBoolean(result);
            }

            return false;
=======
                new SqlParameter("@RoleCode", SqlDbType.NVarChar, 100)
                {
                    Value = string.IsNullOrWhiteSpace(roleCode) ? DBNull.Value : roleCode
                }
            };

            object? scalarResult = _bdl.ExecuteScalar("sp_CheckDuplicateRoleCode", sqlprms);

            return ParseBooleanResult(scalarResult);
>>>>>>> 5-rolelist
        }

        public bool IsRoleNameDuplicate(string roleName)
        {
            SqlParameter[] sqlprms =
            {
<<<<<<< HEAD
                new SqlParameter("@RoleCode", (object?)roleName ?? string.Empty)
            };

            object result = _bdl.ExecuteScalar("sp_CheckDuplicateRoleCode", sqlprms);

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToBoolean(result);
            }

            return false;
=======
                new SqlParameter("@RoleCode", SqlDbType.NVarChar, 100)
                {
                    Value = string.IsNullOrWhiteSpace(roleName) ? DBNull.Value : roleName
                }
            };

            object? scalarResult = _bdl.ExecuteScalar("sp_CheckDuplicateRoleCode", sqlprms);

            return ParseBooleanResult(scalarResult);
>>>>>>> 5-rolelist
        }

        public DataTable GetRoleByCode(string roleCode)
        {
<<<<<<< HEAD
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@RoleCode", (object?)roleCode ?? string.Empty)
            };

            return _bdl.ExecuteDataTable("sp_GetRoleByCode", sqlprms);
=======
            SqlParameter[] sqlprms = { new SqlParameter("@RoleCode", (object?)roleCode ?? string.Empty) };
            return _bdl.SelectData("sp_GetRoleByCode", sqlprms);
>>>>>>> 5-rolelist
        }

        public DataTable GetRolePermissionsByCode(string roleCode)
        {
            DataTable dtAllMenus = GetAllMenus();

<<<<<<< HEAD
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@RoleCode", (object?)roleCode ?? string.Empty)
            };

            DataTable dtRolePerms = _bdl.ExecuteDataTable("sp_GetRolePermission", sqlprms);
=======
            SqlParameter[] sqlprms = { new SqlParameter("@RoleCode", (object?)roleCode ?? string.Empty) };
            DataTable dtRolePerms = _bdl.SelectData("sp_GetRolePermission", sqlprms);
>>>>>>> 5-rolelist
            StandardizeMenuColumns(dtRolePerms);

            var permDict = new Dictionary<int, (bool Read, bool Write, bool Delete)>();
            if (dtRolePerms != null && dtRolePerms.Rows.Count > 0)
            {
                foreach (DataRow row in dtRolePerms.Rows)
                {
<<<<<<< HEAD
                    int menuId = Convert.ToInt32(row["MenuId"] ?? 0);
                    bool canRead = dtRolePerms.Columns.Contains("CanRead") && Convert.ToBoolean(row["CanRead"]);
                    bool canWrite = dtRolePerms.Columns.Contains("CanWrite") && Convert.ToBoolean(row["CanWrite"]);
                    bool canDelete = dtRolePerms.Columns.Contains("CanDelete") && Convert.ToBoolean(row["CanDelete"]);
                    permDict[menuId] = (canRead, canWrite, canDelete);
=======
                    int menuId = row["MenuId"] != DBNull.Value ? Convert.ToInt32(row["MenuId"]) : 0;
                    bool canRead = dtRolePerms.Columns.Contains("CanRead") && row["CanRead"] != DBNull.Value && Convert.ToBoolean(row["CanRead"]);
                    bool canWrite = dtRolePerms.Columns.Contains("CanWrite") && row["CanWrite"] != DBNull.Value && Convert.ToBoolean(row["CanWrite"]);
                    bool canDelete = dtRolePerms.Columns.Contains("CanDelete") && row["CanDelete"] != DBNull.Value && Convert.ToBoolean(row["CanDelete"]);

                    if (menuId > 0)
                    {
                        permDict[menuId] = (canRead, canWrite, canDelete);
                    }
>>>>>>> 5-rolelist
                }
            }

            if (!dtAllMenus.Columns.Contains("CanRead")) dtAllMenus.Columns.Add("CanRead", typeof(bool));
            if (!dtAllMenus.Columns.Contains("CanWrite")) dtAllMenus.Columns.Add("CanWrite", typeof(bool));
            if (!dtAllMenus.Columns.Contains("CanDelete")) dtAllMenus.Columns.Add("CanDelete", typeof(bool));

            foreach (DataRow row in dtAllMenus.Rows)
            {
<<<<<<< HEAD
                int menuId = Convert.ToInt32(row["MenuId"] ?? 0);
=======
                int menuId = row["MenuId"] != DBNull.Value ? Convert.ToInt32(row["MenuId"]) : 0;
>>>>>>> 5-rolelist
                if (permDict.TryGetValue(menuId, out var p))
                {
                    row["CanRead"] = p.Read;
                    row["CanWrite"] = p.Write;
                    row["CanDelete"] = p.Delete;
                }
                else
                {
                    row["CanRead"] = false;
                    row["CanWrite"] = false;
                    row["CanDelete"] = false;
                }
            }

            return dtAllMenus;
        }

        public DataTable GetAllMenus()
        {
<<<<<<< HEAD
            DataTable dt = _bdl.ExecuteDataTable("sp_GetMenuList");
=======
            DataTable dt = _bdl.SelectData("sp_GetMenuList");
>>>>>>> 5-rolelist
            StandardizeMenuColumns(dt);

            if (dt == null || dt.Rows.Count == 0 || !dt.Columns.Contains("MenuId") || !dt.Columns.Contains("ParentId"))
            {
                return dt ?? new DataTable();
            }

            if (!dt.Columns.Contains("Level"))
            {
                dt.Columns.Add("Level", typeof(int));
            }

            var rowsList = dt.AsEnumerable().ToList();
            DataTable sortedDt = dt.Clone();
            HashSet<int> addedMenuIds = new HashSet<int>();

<<<<<<< HEAD
            var rootMenus = rowsList.Where(r => r["ParentId"] == DBNull.Value || Convert.ToInt32(r["ParentId"]) == 0)
=======
            var rootMenus = rowsList.Where(r => r["ParentId"] == DBNull.Value || (r["ParentId"] != DBNull.Value && Convert.ToInt32(r["ParentId"]) == 0))
>>>>>>> 5-rolelist
                                    .OrderBy(r => GetDisplayOrder(r))
                                    .ToList();

            foreach (var root in rootMenus)
            {
                AppendMenuAndChildren(root, rowsList, sortedDt, addedMenuIds, 0);
            }

            var remainingMenus = rowsList.Where(r => !addedMenuIds.Contains(Convert.ToInt32(r["MenuId"]))).ToList();
            foreach (var rem in remainingMenus)
            {
                rem["Level"] = 0;
                sortedDt.ImportRow(rem);
            }

            return sortedDt;
        }

<<<<<<< HEAD
=======
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

                    int level = 0;
                    if (row.Table.Columns.Contains("Level") && row["Level"] != DBNull.Value)
                    {
                        level = Convert.ToInt32(row["Level"]);
                    }

                    list.Add(new MenuPermissionViewModel
                    {
                        MenuId = menuId,
                        MenuName = row["MenuName"]?.ToString() ?? string.Empty,
                        ParentId = parentId,
                        Level = level,
                        CanRead = row.Table.Columns.Contains("CanRead") && row["CanRead"] != DBNull.Value && Convert.ToBoolean(row["CanRead"]),
                        CanWrite = row.Table.Columns.Contains("CanWrite") && row["CanWrite"] != DBNull.Value && Convert.ToBoolean(row["CanWrite"]),
                        CanDelete = row.Table.Columns.Contains("CanDelete") && row["CanDelete"] != DBNull.Value && Convert.ToBoolean(row["CanDelete"])
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

        private static bool ParseBooleanResult(object? scalarResult)
        {
            if (scalarResult != null && scalarResult != DBNull.Value)
            {
                if (scalarResult is bool b) return b;
                if (int.TryParse(scalarResult.ToString(), out int val)) return val > 0;
                return scalarResult.ToString()!.Equals("true", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private static bool ParseStatus(object? statusObj)
        {
            if (statusObj == null || statusObj == DBNull.Value) return false;
            if (statusObj is bool b) return b;
            if (int.TryParse(statusObj.ToString(), out int val)) return val == 1;
            string str = statusObj.ToString()!.Trim();
            return str.Equals("true", StringComparison.OrdinalIgnoreCase) || str == "1";
        }

>>>>>>> 5-rolelist
        private static void AppendMenuAndChildren(DataRow currentMenu, List<DataRow> allRows, DataTable targetTable, HashSet<int> addedIds, int currentLevel)
        {
            int currentId = Convert.ToInt32(currentMenu["MenuId"]);
            if (addedIds.Contains(currentId)) return;

            currentMenu["Level"] = currentLevel;
            targetTable.ImportRow(currentMenu);
            addedIds.Add(currentId);

            var children = allRows.Where(r => r["ParentId"] != DBNull.Value && Convert.ToInt32(r["ParentId"]) == currentId)
                                  .OrderBy(r => GetDisplayOrder(r))
                                  .ToList();

            foreach (var child in children)
            {
                AppendMenuAndChildren(child, allRows, targetTable, addedIds, currentLevel + 1);
            }
        }

        private static int GetDisplayOrder(DataRow row)
        {
            if (row.Table.Columns.Contains("DisplayOrder") && row["DisplayOrder"] != DBNull.Value)
            {
                return Convert.ToInt32(row["DisplayOrder"]);
            }
            return 0;
        }

        private static void StandardizeMenuColumns(DataTable? dt)
        {
            if (dt == null) return;

            RenameColumnIfExist(dt, new[] { "MenuID", "Menu_Id", "Menu_ID" }, "MenuId");
            RenameColumnIfExist(dt, new[] { "ParentMenuId", "Parent_Menu_Id", "ParentMenuID", "Parent_Menu_ID", "Parent_Id", "Parent_ID" }, "ParentId");
        }

        private static void RenameColumnIfExist(DataTable dt, string[] possibleNames, string targetName)
        {
            if (dt.Columns.Contains(targetName)) return;

            foreach (var name in possibleNames)
            {
                if (dt.Columns.Contains(name))
                {
                    dt.Columns[name].ColumnName = targetName;
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
<<<<<<< HEAD
=======

        #endregion
>>>>>>> 5-rolelist
    }
}
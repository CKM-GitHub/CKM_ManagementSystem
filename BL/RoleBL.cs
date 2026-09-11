using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Roles;
using Microsoft.Data.SqlClient;

namespace CKM_ManagementSystem.BL
{
    public class RoleBL
    {
        private readonly BaseDL _bdl;

        public RoleBL(BaseDL baseDL)
        {
            _bdl = baseDL ?? throw new ArgumentNullException(nameof(baseDL));
        }

        public string Role_Insert(Roles role, List<RolePermission> permissions)
        {
            return SaveRoleInfo(role, permissions);
        }

        public string Role_Update(Roles role, List<RolePermission> permissions)
        {
            return SaveRoleInfo(role, permissions);
        }

        private string SaveRoleInfo(Roles role, List<RolePermission> permissions)
        {
            DataTable dtPermissions = ConvertPermissionsToDataTable(permissions);

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

        public bool IsRoleCodeDuplicate(string roleCode)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@RoleCode", (object?)roleCode ?? string.Empty)
            };

            object result = _bdl.ExecuteScalar("sp_CheckDuplicateRoleCode", sqlprms);

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToBoolean(result);
            }

            return false;
        }

        public bool IsRoleNameDuplicate(string roleName)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@RoleCode", (object?)roleName ?? string.Empty)
            };

            object result = _bdl.ExecuteScalar("sp_CheckDuplicateRoleCode", sqlprms);

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToBoolean(result);
            }

            return false;
        }

        public DataTable GetRoleByCode(string roleCode)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@RoleCode", (object?)roleCode ?? string.Empty)
            };

            return _bdl.ExecuteDataTable("sp_GetRoleByCode", sqlprms);
        }

        public DataTable GetRolePermissionsByCode(string roleCode)
        {
            DataTable dtAllMenus = GetAllMenus();

            SqlParameter[] sqlprms =
            {
                new SqlParameter("@RoleCode", (object?)roleCode ?? string.Empty)
            };

            DataTable dtRolePerms = _bdl.ExecuteDataTable("sp_GetRolePermission", sqlprms);
            StandardizeMenuColumns(dtRolePerms);

            var permDict = new Dictionary<int, (bool Read, bool Write, bool Delete)>();
            if (dtRolePerms != null && dtRolePerms.Rows.Count > 0)
            {
                foreach (DataRow row in dtRolePerms.Rows)
                {
                    int menuId = Convert.ToInt32(row["MenuId"] ?? 0);
                    bool canRead = dtRolePerms.Columns.Contains("CanRead") && Convert.ToBoolean(row["CanRead"]);
                    bool canWrite = dtRolePerms.Columns.Contains("CanWrite") && Convert.ToBoolean(row["CanWrite"]);
                    bool canDelete = dtRolePerms.Columns.Contains("CanDelete") && Convert.ToBoolean(row["CanDelete"]);
                    permDict[menuId] = (canRead, canWrite, canDelete);
                }
            }

            if (!dtAllMenus.Columns.Contains("CanRead")) dtAllMenus.Columns.Add("CanRead", typeof(bool));
            if (!dtAllMenus.Columns.Contains("CanWrite")) dtAllMenus.Columns.Add("CanWrite", typeof(bool));
            if (!dtAllMenus.Columns.Contains("CanDelete")) dtAllMenus.Columns.Add("CanDelete", typeof(bool));

            foreach (DataRow row in dtAllMenus.Rows)
            {
                int menuId = Convert.ToInt32(row["MenuId"] ?? 0);
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
            DataTable dt = _bdl.ExecuteDataTable("sp_GetMenuList");
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

            var rootMenus = rowsList.Where(r => r["ParentId"] == DBNull.Value || Convert.ToInt32(r["ParentId"]) == 0)
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

        public RoleListPagedViewModel GetRoleListPaged(
    int pageNumber,
    int pageSize,
    string searchKeyword,
    int? status)
        {
            SqlParameter[] sqlprms =
            {
        new SqlParameter(
            "@SearchKeyword",
            string.IsNullOrWhiteSpace(searchKeyword)
                ? (object)DBNull.Value
                : searchKeyword),

        new SqlParameter(
            "@Status",
            status.HasValue
                ? (object)status.Value
                : DBNull.Value),

        new SqlParameter(
            "@Offset",
            (pageNumber - 1) * pageSize),

        new SqlParameter(
            "@PageSize",
            pageSize)
    };

            DataSet ds =
                _bdl.SelectDataSet(
                    "sp_GetRoleListPaged",
                    sqlprms);

            DataTable dtData =
                ds.Tables.Count > 0
                    ? ds.Tables[0]
                    : new DataTable();

            int totalRecords = 0;

            if (ds.Tables.Count > 1 &&
                ds.Tables[1].Rows.Count > 0)
            {
                totalRecords =
                    Convert.ToInt32(
                        ds.Tables[1].Rows[0][0]);
            }

            var roles =
                new List<RoleEntryViewModel>();

            foreach (DataRow row in dtData.Rows)
            {
                roles.Add(new RoleEntryViewModel
                {
                    RoleCode =
                        row["RoleCode"]?.ToString() ?? "",

                    DisplayName =
                        row["DisplayName"]?.ToString() ?? "",

                    Description =
                        row["Description"] == DBNull.Value
                            ? ""
                            : row["Description"]?.ToString() ?? "",

                    Status =
                        ParseStatus(row["Status"])
                });
            }

            return new RoleListPagedViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchKeyword = searchKeyword ?? string.Empty,
                Status = status,
                TotalRecords = totalRecords,
                Roles = roles
            };
        }
        public RoleEntryViewModel? GetRoleByCodeViewModel(
            string roleCode)
        {
            DataTable dt =
                GetRoleByCode(roleCode);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dt.Rows[0];

            string code =
                dt.Columns.Contains("RoleCode")
                    ? row["RoleCode"]?.ToString() ?? ""
                    : dt.Columns.Contains("Role_Code")
                        ? row["Role_Code"]?.ToString() ?? ""
                        : "";

            string name =
                dt.Columns.Contains("DisplayName")
                    ? row["DisplayName"]?.ToString() ?? ""
                    : dt.Columns.Contains("Role_Name")
                        ? row["Role_Name"]?.ToString() ?? ""
                        : "";

            string description =
                dt.Columns.Contains("Description") &&
                row["Description"] != DBNull.Value
                    ? row["Description"]?.ToString() ?? ""
                    : "";

            return new RoleEntryViewModel
            {
                RoleCode = code,
                DisplayName = name,
                Description = description,
                Status =
                    dt.Columns.Contains("Status")
                        ? ParseStatus(row["Status"])
                        : false
            };
        }

        public (bool Success, string Message) DeleteRole(
            string roleCode)
        {
            var role =
                GetRoleByCodeViewModel(roleCode);

            if (role == null)
            {
                return (
                    false,
                    "Role not found.");
            }

            if (role.Status)
            {
                return (
                    false,
                    "Cannot delete an active role.");
            }

            SqlParameter[] prms =
            {
        new SqlParameter(
            "@RoleCode",
            (object?)roleCode ?? DBNull.Value)
    };

            string result =
                _bdl.InsertUpdateDeleteData(
                    "sp_DeleteRole",
                    prms);

            if (string.IsNullOrEmpty(result) ||
                result.Equals(
                    "true",
                    StringComparison.OrdinalIgnoreCase) ||
                result == "1")
            {
                return (
                    true,
                    "Role deleted successfully.");
            }

            return (
                false,
                "An error occurred while deleting the role: "
                + result);
        }

        private static bool ParseStatus(
            object? statusObj)
        {
            if (statusObj == null ||
                statusObj == DBNull.Value)
            {
                return false;
            }

            if (statusObj is bool status)
            {
                return status;
            }

            if (int.TryParse(
                statusObj.ToString(),
                out int value))
            {
                return value == 1;
            }

            string text =
                statusObj.ToString()!.Trim();

            return text.Equals(
                       "true",
                       StringComparison.OrdinalIgnoreCase)
                   || text == "1";
        }

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
    }
}
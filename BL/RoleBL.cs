using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.Entities;
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

            return _bdl.SelectData("sp_GetRoleByCode", sqlprms);
        }

        public DataTable GetRolePermissionsByCode(string roleCode)
        {
            DataTable dtAllMenus = GetAllMenus();

            SqlParameter[] sqlprms =
            {
                new SqlParameter("@RoleCode", (object?)roleCode ?? string.Empty)
            };

            DataTable dtRolePerms = _bdl.SelectData("sp_GetRolePermission", sqlprms);
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
            DataTable dt = _bdl.SelectData("sp_GetMenuList");
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
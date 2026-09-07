using System;
using System.Collections.Generic;
using System.Data;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.Entities;
using Microsoft.Data.SqlClient;

namespace CKM_ManagementSystem.BL
{
    public class RoleBL
    {
        private readonly BaseDL bdl;

        public RoleBL(BaseDL baseDL)
        {
            bdl = baseDL;
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
                new SqlParameter("@Role_Code", (object)role.RoleCode ?? string.Empty),
                new SqlParameter("@Role_Name", (object)role.RoleName ?? string.Empty),
                new SqlParameter("@Description", (object)role.Description ?? DBNull.Value),
                new SqlParameter("@Status", role.Status),
                paramPermissions
            };

            return bdl.InsertUpdateDeleteData("sp_SaveRoleInfo", sqlprms);
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

        private static void StandardizeMenuColumns(DataTable dt)
        {
            if (dt == null) return;

            // Target standardization dictionary (Original variations -> Normalized Name)
            var columnMappings = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                { "ParentId", new[] { "ParentMenuId", "Parent_Menu_Id", "ParentMenuID", "Parent_Menu_ID", "Parent_Id", "Parent_ID" } },
                { "MenuId", new[] { "MenuID", "Menu_Id", "Menu_ID" } }
            };

            foreach (var mapping in columnMappings)
            {
                string targetCol = mapping.Key;
                if (!dt.Columns.Contains(targetCol))
                {
                    foreach (var possibleCol in mapping.Value)
                    {
                        if (dt.Columns.Contains(possibleCol))
                        {
                            dt.Columns[possibleCol].ColumnName = targetCol;
                            break;
                        }
                    }
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
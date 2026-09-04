using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Roles;

namespace CKM_ManagementSystem.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleBL _roleBL;

        public RoleController(RoleBL roleBL)
        {
            _roleBL = roleBL;
        }

        [HttpGet]
        public IActionResult RoleEntry(string? id)
        {
            var model = new RoleEntryViewModel();
            List<RolePermissionViewModel> rawPermissions = new List<RolePermissionViewModel>();

            if (!string.IsNullOrEmpty(id))
            {
                DataTable dtRole = _roleBL.GetRoleByCode(id);
                if (dtRole != null && dtRole.Rows.Count > 0)
                {
                    DataRow row = dtRole.Rows[0];
                    model.RoleCode = row["Role_Code"]?.ToString() ?? string.Empty;
                    model.DisplayName = row["Role_Name"]?.ToString() ?? string.Empty;
                    model.Description = row["Description"] != DBNull.Value ? row["Description"]?.ToString() : null;
                    model.Status = row["Status"] != DBNull.Value && Convert.ToBoolean(row["Status"]);
                }

                DataTable dtPermissions = _roleBL.GetRolePermissionsByCode(id);
                rawPermissions = MapDataTableToMenuPermissionList(dtPermissions);
            }
            else
            {
                DataTable dtMenus = _roleBL.GetAllMenus();
                rawPermissions = MapDataTableToMenuPermissionList(dtMenus);
            }

            model.MenuPermissions = SortMenuHierarchy(rawPermissions);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RoleEntry(RoleEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please fill in all required fields properly." });
            }

            var role = new Roles
            {
                RoleCode = model.RoleCode,
                RoleName = model.DisplayName,
                Description = model.Description,
                Status = model.Status
            };

            var permissions = model.MenuPermissions?.Select(p => new RolePermission
            {
                MenuId = p.MenuId,
                CanRead = p.CanRead,
                CanWrite = p.CanWrite,
                CanDelete = p.CanDelete
            }).ToList() ?? new List<RolePermission>();

            string result = _roleBL.Role_Insert(role, permissions);

            
            if (string.Equals(result, "true", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(result, "SUCCESS", StringComparison.OrdinalIgnoreCase) ||
                result.Contains("successfully", StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { success = true, message = "Role saved successfully!" });
            }

            return Json(new { success = false, message = result });
        }

        private List<RolePermissionViewModel> MapDataTableToMenuPermissionList(DataTable dt)
        {
            var list = new List<RolePermissionViewModel>();
            if (dt == null) return list;

            foreach (DataRow row in dt.Rows)
            {
                int? parentId = null;

                string parentColName = dt.Columns.Contains("ParentMenuId") ? "ParentMenuId" :
                                       (dt.Columns.Contains("Parent_Menu_Id") ? "Parent_Menu_Id" :
                                       (dt.Columns.Contains("ParentId") ? "ParentId" : null));

                if (parentColName != null && row[parentColName] != DBNull.Value)
                {
                    if (int.TryParse(row[parentColName].ToString(), out int parsedParentId) && parsedParentId > 0)
                    {
                        parentId = parsedParentId;
                    }
                }

                list.Add(new RolePermissionViewModel
                {
                    MenuId = Convert.ToInt32(row["MenuId"]),
                    MenuName = row["MenuName"]?.ToString() ?? string.Empty,
                    ParentId = parentId,
                    CanRead = dt.Columns.Contains("CanRead") && row["CanRead"] != DBNull.Value && Convert.ToBoolean(row["CanRead"]),
                    CanWrite = dt.Columns.Contains("CanWrite") && row["CanWrite"] != DBNull.Value && Convert.ToBoolean(row["CanWrite"]),
                    CanDelete = dt.Columns.Contains("CanDelete") && row["CanDelete"] != DBNull.Value && Convert.ToBoolean(row["CanDelete"])
                });
            }

            return list;
        }

        private List<RolePermissionViewModel> SortMenuHierarchy(List<RolePermissionViewModel> rawList)
        {
            if (rawList == null || !rawList.Any()) return new List<RolePermissionViewModel>();

            var sortedList = new List<RolePermissionViewModel>();
            var rootMenus = rawList
                .Where(m => !m.ParentId.HasValue || m.ParentId.Value == 0)
                .OrderBy(m => m.MenuId)
                .ToList();

            foreach (var root in rootMenus)
            {
                AddMenuAndChildren(root, rawList, sortedList, 0);
            }

            var addedIds = sortedList.Select(s => s.MenuId).ToHashSet();
            var orphanMenus = rawList.Where(m => !addedIds.Contains(m.MenuId)).ToList();

            foreach (var orphan in orphanMenus)
            {
                orphan.ParentId = null;
                orphan.Level = 0;
                sortedList.Add(orphan);
            }

            return sortedList;
        }

        private void AddMenuAndChildren(RolePermissionViewModel currentMenu, List<RolePermissionViewModel> rawList, List<RolePermissionViewModel> resultList, int currentLevel)
        {
            currentMenu.Level = currentLevel;
            resultList.Add(currentMenu);

            var children = rawList
                .Where(m => m.ParentId.HasValue && m.ParentId.Value == currentMenu.MenuId)
                .OrderBy(m => m.MenuId)
                .ToList();

            foreach (var child in children)
            {
                AddMenuAndChildren(child, rawList, resultList, currentLevel + 1);
            }
        }
    }
}
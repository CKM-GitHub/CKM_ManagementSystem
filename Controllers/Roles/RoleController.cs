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
            List<RolePermissionViewModel> rawPermissions;

            if (!string.IsNullOrEmpty(id))
            {
                model.IsEdit = true;

                DataTable dtRole = _roleBL.GetRoleByCode(id);

                if (dtRole != null && dtRole.Rows.Count > 0)
                {
                    DataRow row = dtRole.Rows[0];

                    model.RoleCode =
                        GetColumnValue(row, "Role_Code", "RoleCode");

                    model.DisplayName =
                        GetColumnValue(
                            row,
                            "Role_Name",
                            "RoleName",
                            "DisplayName");

                    model.Description =
                        GetColumnObject(row, "Description")?.ToString();

                    model.Status =
                        GetBooleanValue(row, "Status");
                }

                DataTable dtPermissions =
                    _roleBL.GetRolePermissionsByCode(id);

                rawPermissions =
                    MapDataTableToMenuPermissionList(dtPermissions);
            }
            else
            {
                model.IsEdit = false;

                DataTable dtMenus =
                    _roleBL.GetAllMenus();

                rawPermissions =
                    MapDataTableToMenuPermissionList(dtMenus);
            }

            model.MenuPermissions =
                SortMenuHierarchy(rawPermissions);

            return View(model);
        }

        [HttpGet]
        public IActionResult RoleList(
            string searchKeyword,
            int? status,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var model =
                _roleBL.GetRoleListPaged(
                    pageNumber,
                    pageSize,
                    searchKeyword,
                    status);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRole(string roleCode)
        {
            if (string.IsNullOrWhiteSpace(roleCode))
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid Role Code."
                });
            }

            var result =
                _roleBL.DeleteRole(roleCode);

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RoleEntry(
            RoleEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message =
                        "Please fill in all required fields properly."
                });
            }

            if (!model.IsEdit &&
                _roleBL.IsRoleCodeDuplicate(model.RoleCode))
            {
                return Json(new
                {
                    success = false,
                    message =
                        $"Role Code '{model.RoleCode}' already exists."
                });
            }

            bool checkDuplicateName = true;

            if (model.IsEdit)
            {
                var existingRole =
                    _roleBL.GetRoleByCodeViewModel(
                        model.RoleCode);

                if (existingRole != null &&
                    string.Equals(
                        existingRole.DisplayName,
                        model.DisplayName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    checkDuplicateName = false;
                }
            }

            if (checkDuplicateName &&
                _roleBL.IsRoleNameDuplicate(
                    model.DisplayName))
            {
                return Json(new
                {
                    success = false,
                    message =
                        $"Role Name '{model.DisplayName}' already exists."
                });
            }

            if (model.MenuPermissions != null &&
                model.MenuPermissions.Count > 0)
            {
                foreach (var perm in
                         model.MenuPermissions)
                {
                    if (perm.CanWrite ||
                        perm.CanDelete)
                    {
                        perm.CanRead = true;
                    }
                }
            }

            var role = new Roles
            {
                RoleCode = model.RoleCode,
                RoleName = model.DisplayName,
                Description = model.Description,
                Status = model.Status
            };

            var permissions =
                model.MenuPermissions?
                    .Select(p => new RolePermission
                    {
                        MenuId = p.MenuId,
                        CanRead = p.CanRead,
                        CanWrite = p.CanWrite,
                        CanDelete = p.CanDelete
                    })
                    .ToList()
                ?? new List<RolePermission>();

            string result;

            if (model.IsEdit)
            {
                result =
                    _roleBL.Role_Update(
                        role,
                        permissions);
            }
            else
            {
                result =
                    _roleBL.Role_Insert(
                        role,
                        permissions);
            }

            if (string.Equals(
                    result,
                    "true",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    result,
                    "SUCCESS",
                    StringComparison.OrdinalIgnoreCase) ||
                result.Contains(
                    "successfully",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Json(new
                {
                    success = true,
                    message = model.IsEdit
                        ? "Role updated successfully!"
                        : "Role saved successfully!"
                });
            }

            return Json(new
            {
                success = false,
                message =
                    !string.IsNullOrEmpty(result)
                        ? result
                        : "Failed to save role into database."
            });
        }

        #region Private Mapping & Hierarchy Methods

        private List<RolePermissionViewModel>
            MapDataTableToMenuPermissionList(
                DataTable dt)
        {
            var list =
                new List<RolePermissionViewModel>();

            if (dt == null ||
                dt.Rows.Count == 0)
            {
                return list;
            }

            string[] parentCols =
            {
                "ParentMenuId",
                "Parent_Menu_Id",
                "ParentId",
                "Parent_Id",
                "Parent_Menu_ID",
                "Parent_ID",
                "MenuParentId",
                "Menu_Parent_Id"
            };

            string? parentColName =
                parentCols.FirstOrDefault(
                    col => dt.Columns.Contains(col));

            foreach (DataRow row in dt.Rows)
            {
                int? parentId = null;

                if (parentColName != null &&
                    row[parentColName] != DBNull.Value)
                {
                    if (int.TryParse(
                            row[parentColName].ToString(),
                            out int parsedParentId) &&
                        parsedParentId > 0)
                    {
                        parentId = parsedParentId;
                    }
                }

                int menuId =
                    Convert.ToInt32(
                        GetColumnObject(
                            row,
                            "MenuId",
                            "Menu_Id",
                            "ID") ?? 0);

                string menuName =
                    GetColumnValue(
                        row,
                        "MenuName",
                        "Menu_Name",
                        "Name");

                list.Add(
                    new RolePermissionViewModel
                    {
                        MenuId = menuId,
                        MenuName = menuName,
                        ParentId = parentId,
                        CanRead =
                            GetBooleanValue(
                                row,
                                "CanRead",
                                "Can_Read"),
                        CanWrite =
                            GetBooleanValue(
                                row,
                                "CanWrite",
                                "Can_Write"),
                        CanDelete =
                            GetBooleanValue(
                                row,
                                "CanDelete",
                                "Can_Delete")
                    });
            }

            return list;
        }

        private List<RolePermissionViewModel>
            SortMenuHierarchy(
                List<RolePermissionViewModel> rawList)
        {
            if (rawList == null ||
                !rawList.Any())
            {
                return new List<RolePermissionViewModel>();
            }

            var sortedList =
                new List<RolePermissionViewModel>();

            var rootMenus =
                rawList
                    .Where(m =>
                        !m.ParentId.HasValue ||
                        m.ParentId.Value == 0)
                    .OrderBy(m => m.MenuId)
                    .ToList();

            foreach (var root in rootMenus)
            {
                AddMenuAndChildren(
                    root,
                    rawList,
                    sortedList,
                    0);
            }

            var addedIds =
                sortedList
                    .Select(s => s.MenuId)
                    .ToHashSet();

            var orphanMenus =
                rawList
                    .Where(m =>
                        !addedIds.Contains(m.MenuId))
                    .ToList();

            foreach (var orphan in orphanMenus)
            {
                orphan.ParentId = null;
                orphan.Level = 0;

                sortedList.Add(orphan);
            }

            return sortedList;
        }

        private void AddMenuAndChildren(
            RolePermissionViewModel currentMenu,
            List<RolePermissionViewModel> rawList,
            List<RolePermissionViewModel> resultList,
            int currentLevel)
        {
            currentMenu.Level = currentLevel;

            resultList.Add(currentMenu);

            var children =
                rawList
                    .Where(m =>
                        m.ParentId.HasValue &&
                        m.ParentId.Value ==
                        currentMenu.MenuId)
                    .OrderBy(m => m.MenuId)
                    .ToList();

            foreach (var child in children)
            {
                AddMenuAndChildren(
                    child,
                    rawList,
                    resultList,
                    currentLevel + 1);
            }
        }

        #endregion

        #region Helper Methods for DataTable Columns

        private object? GetColumnObject(
            DataRow row,
            params string[] columnNames)
        {
            foreach (var name in columnNames)
            {
                if (row.Table.Columns.Contains(name) &&
                    row[name] != DBNull.Value)
                {
                    return row[name];
                }
            }

            return null;
        }

        private string GetColumnValue(
            DataRow row,
            params string[] columnNames)
        {
            var obj =
                GetColumnObject(
                    row,
                    columnNames);

            return obj?.ToString()
                   ?? string.Empty;
        }

        private bool GetBooleanValue(
            DataRow row,
            params string[] columnNames)
        {
            var obj =
                GetColumnObject(
                    row,
                    columnNames);

            if (obj != null)
            {
                string val =
                    obj.ToString()!.Trim();

                if (bool.TryParse(
                        val,
                        out bool result))
                {
                    return result;
                }

                return val == "1" ||
                       val.Equals(
                           "true",
                           StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        #endregion
    }
}
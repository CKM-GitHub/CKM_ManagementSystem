using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Roles;

namespace CKM_ManagementSystem.Controllers.Roles
{
    public class RoleController : Controller
    {
        private readonly RoleBL _roleBL;

        public RoleController(RoleBL roleBL)
        {
            _roleBL = roleBL;
        }

        [HttpGet]
        public IActionResult RoleEntry(string? roleCode)
        {
            var model = new RoleEntryViewModel
            {
                MenuPermissions = new List<RolePermissionViewModel>()
            };

            bool isEdit = !string.IsNullOrEmpty(roleCode);

            if (isEdit)
            {
                DataTable dtRole = _roleBL.GetRoleByCode(roleCode!);
                if (dtRole != null && dtRole.Rows.Count > 0)
                {
                    DataRow row = dtRole.Rows[0];
                    model.RoleCode = row["Role_Code"]?.ToString() ?? "";
                    model.DisplayName = row["Role_Name"]?.ToString() ?? "";
                    model.Description = row["Description"]?.ToString() ?? "";
                    model.Status = row["Status"] != DBNull.Value && Convert.ToBoolean(row["Status"]);
                }

                DataTable dtPermissions = _roleBL.GetRolePermissionsByCode(roleCode!);
                model.MenuPermissions = MapDataTableToPermissions(dtPermissions);
            }
            else
            {
                DataTable dtMenus = _roleBL.GetAllMenus();
                model.MenuPermissions = MapDataTableToPermissions(dtMenus);
            }

            model.MenuPermissions = SortMenuHierarchy(model.MenuPermissions);
            ViewBag.IsEdit = isEdit;

            return View(model);
        }

        [HttpPost]
        public IActionResult CheckDuplicateCode(string roleCode)
        {
            if (string.IsNullOrWhiteSpace(roleCode))
            {
                return Json(new { isDuplicate = false });
            }

            bool isDuplicate = _roleBL.IsRoleCodeDuplicate(roleCode);
            return Json(new { isDuplicate = isDuplicate });
        }

        [HttpPost]
        public IActionResult SaveRole(RoleEntryViewModel model, bool isEdit = false)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, message = "Validation Error: " + string.Join(", ", errors) });
            }

            try
            {
                Roles role = new Roles
                {
                    RoleCode = model.RoleCode,
                    RoleName = model.DisplayName,
                    Description = model.Description,
                    Status = model.Status
                };

                List<RolePermission> permissions = new List<RolePermission>();
                if (model.MenuPermissions != null)
                {
                    foreach (var p in model.MenuPermissions)
                    {
                        permissions.Add(new RolePermission
                        {
                            RoleCode = model.RoleCode,
                            MenuId = p.MenuId,
                            CanRead = p.CanRead,
                            CanWrite = p.CanWrite,
                            CanDelete = p.CanDelete
                        });
                    }
                }

                string result;
                if (isEdit)
                {
                    result = _roleBL.Role_Update(role, permissions);
                }
                else
                {
                    if (_roleBL.IsRoleCodeDuplicate(model.RoleCode))
                    {
                        return Json(new { success = false, message = "This Role Code already exists." });
                    }
                    result = _roleBL.Role_Insert(role, permissions);
                }

                if (result.ToLower() == "true" || result == "1")
                {
                    return Json(new
                    {
                        success = true,
                        isEdit = isEdit,
                        message = isEdit ? "Update is complete." : "Registration is complete."
                    });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while saving data: " + ex.Message });
            }
        }

        #region Helper Methods

        private static List<RolePermissionViewModel> SortMenuHierarchy(List<RolePermissionViewModel> rawList)
        {
            if (rawList == null || !rawList.Any())
                return new List<RolePermissionViewModel>();

            var sortedList = new List<RolePermissionViewModel>();

            var mainMenus = rawList.Where(m => m.ParentId == null || m.ParentId == 0).ToList();

            foreach (var main in mainMenus)
            {
                sortedList.Add(main);

                var subMenus = rawList.Where(m => m.ParentId == main.MenuId).ToList();
                sortedList.AddRange(subMenus);
            }

            var orphanMenus = rawList.Except(sortedList).ToList();
            if (orphanMenus.Any())
            {
                sortedList.AddRange(orphanMenus);
            }

            return sortedList;
        }

        private static List<RolePermissionViewModel> MapDataTableToPermissions(DataTable dt)
        {
            var list = new List<RolePermissionViewModel>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    int? parentId = null;

                    if (row.Table.Columns.Contains("ParentMenuId") && row["ParentMenuId"] != DBNull.Value)
                    {
                        parentId = Convert.ToInt32(row["ParentMenuId"]);
                    }
                    else if (row.Table.Columns.Contains("ParentId") && row["ParentId"] != DBNull.Value)
                    {
                        parentId = Convert.ToInt32(row["ParentId"]);
                    }

                    list.Add(new RolePermissionViewModel
                    {
                        MenuId = row.Table.Columns.Contains("MenuId") ? Convert.ToInt32(row["MenuId"]) : 0,
                        MenuName = row.Table.Columns.Contains("MenuName") ? row["MenuName"]?.ToString() ?? "" : "",
                        ParentId = parentId,
                        CanRead = row.Table.Columns.Contains("CanRead") && Convert.ToBoolean(row["CanRead"]),
                        CanWrite = row.Table.Columns.Contains("CanWrite") && Convert.ToBoolean(row["CanWrite"]),
                        CanDelete = row.Table.Columns.Contains("CanDelete") && Convert.ToBoolean(row["CanDelete"])
                    });
                }
            }
            return list;
        }

        #endregion
    }
}
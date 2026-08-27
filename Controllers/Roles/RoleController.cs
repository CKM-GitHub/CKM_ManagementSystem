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
                return View(model);
            }

            var role = new Roles
            {
                RoleCode = model.RoleCode,
                RoleName = model.DisplayName,
                Description = model.Description,
                Status = model.Status
            };

            var permissions = model.MenuPermissions.Select(p => new RolePermission
            {
                MenuId = p.MenuId,
                CanRead = p.CanRead,
                CanWrite = p.CanWrite,
                CanDelete = p.CanDelete
            }).ToList();

            string result = _roleBL.Role_Insert(role, permissions);

            if (result.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) || result.Contains("successfully", StringComparison.OrdinalIgnoreCase))
            {
                TempData["SuccessMessage"] = "Role saved successfully.";
                return RedirectToAction("RoleList");
            }

            ViewBag.ErrorMessage = result;
            return View(model);
        }

        private List<RolePermissionViewModel> MapDataTableToMenuPermissionList(DataTable dt)
        {
            var list = new List<RolePermissionViewModel>();
            if (dt == null) return list;

            foreach (DataRow row in dt.Rows)
            {
                int? parentId = null;
                if (dt.Columns.Contains("ParentId") && row["ParentId"] != DBNull.Value)
                {
                    int parsedParentId = Convert.ToInt32(row["ParentId"]);
                    if (parsedParentId > 0) parentId = parsedParentId;
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

           
            var mainMenus = rawList.Where(m => !m.ParentId.HasValue || m.ParentId.Value == 0).ToList();

            foreach (var mainMenu in mainMenus)
            {
                sortedList.Add(mainMenu);

               
                var subMenus = rawList.Where(m => m.ParentId.HasValue && m.ParentId.Value == mainMenu.MenuId).ToList();
                sortedList.AddRange(subMenus);
            }

            
            var addedIds = sortedList.Select(s => s.MenuId).ToHashSet();
            var orphanMenus = rawList.Where(m => !addedIds.Contains(m.MenuId)).ToList();
            sortedList.AddRange(orphanMenus);

            return sortedList;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels;
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
        public async Task<IActionResult> RoleList(int pageNumber = 1, int pageSize = 10, string searchKey = "", int? status = null)
        {
            try
            {
                var pagedResult = await _roleBL.GetRoleListPagedAsync(pageNumber, pageSize, searchKey, status);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(new RoleListPagedViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> RoleEntry(string? roleCode)
        {
            var model = new RoleEntryViewModel
            {
                MenuPermissions = new List<RolePermissionViewModel>()
            };

            List<MenuPermissionViewModel> rawPermissions;

            if (!string.IsNullOrEmpty(roleCode))
            {
                var role = await _roleBL.GetRoleByCodeAsync(roleCode);
                if (role != null)
                {
                    model.RoleCode = role.RoleCode;
                    model.DisplayName = role.DisplayName;
                    model.Description = role.Description;
                    model.Status = role.Status;
                }

                rawPermissions = await _roleBL.GetMenuPermissionsAsync(roleCode);
            }
            else
            {
                rawPermissions = await _roleBL.GetMenuPermissionsAsync(null);
            }

            if (rawPermissions != null)
            {
                model.MenuPermissions = rawPermissions.Select(p => new RolePermissionViewModel
                {
                    MenuId = p.MenuId,
                    MenuName = p.MenuName,
                    ParentId = p.ParentId,
                    CanRead = p.CanRead,
                    CanWrite = p.CanWrite,
                    CanDelete = p.CanDelete
                }).ToList();
            }

            model.MenuPermissions = SortMenuHierarchy(model.MenuPermissions);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CheckDuplicateCode(string roleCode)
        {
            bool isDuplicate = await _roleBL.CheckDuplicateRoleCodeSPAsync(roleCode);
            return Json(new { isDuplicate = isDuplicate });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRole(RoleEntryViewModel model, bool isEdit = false)
        {
            if (!ModelState.IsValid)
            {
                return View("RoleEntry", model);
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

            string result;

            if (isEdit)
            {
                result = _roleBL.Role_Update(role, permissions);
            }
            else
            {
                if (await _roleBL.CheckDuplicateRoleCodeSPAsync(model.RoleCode))
                {
                    return Json(new { success = false, message = "This Role Code already exists." });
                }
                result = _roleBL.Role_Insert(role, permissions);
            }

            if (result.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) || result.Contains("successfully", StringComparison.OrdinalIgnoreCase))
            {
                TempData["SuccessMessage"] = "Role saved successfully.";
                return RedirectToAction("RoleList");
            }

            ViewBag.ErrorMessage = result;
            return View("RoleEntry", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleCode)
        {
            try
            {
                var result = await _roleBL.DeleteRoleAsync(roleCode);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #region Helper Methods

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

            var orphanMenus = rawList.Except(sortedList).OrderBy(m => m.MenuId).ToList();
            if (orphanMenus.Any())
            {
                sortedList.AddRange(orphanMenus);
            }

            return sortedList;
        }

        #endregion
    }
}
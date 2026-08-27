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
        public async Task<IActionResult> RoleEntry(string? code, string? roleCode)
        {
            string? targetCode = !string.IsNullOrEmpty(code) ? code : roleCode;

            var model = new RoleEntryViewModel
            {
                MenuPermissions = new List<RolePermissionViewModel>()
            };

            List<MenuPermissionViewModel> rawPermissions;

            if (!string.IsNullOrEmpty(targetCode))
            {
                var role = await _roleBL.GetRoleByCodeAsync(targetCode);
                if (role != null)
                {
                    model.RoleCode = role.RoleCode;
                    model.DisplayName = role.DisplayName;
                    model.Description = role.Description;
                    model.Status = role.Status;
                    model.IsEdit = true;
                }

                rawPermissions = await _roleBL.GetMenuPermissionsAsync(targetCode);
            }
            else
            {
                rawPermissions = await _roleBL.GetMenuPermissionsAsync(null);
            }

            if (rawPermissions != null && rawPermissions.Any())
            {
                var sortedPermissions = SortMenuHierarchy(rawPermissions);

                model.MenuPermissions = sortedPermissions.Select(p => new RolePermissionViewModel
                {
                    MenuId = p.MenuId,
                    MenuName = p.MenuName,
                    ParentId = (p.ParentId.HasValue && p.ParentId.Value > 0) ? p.ParentId : null,
                    CanRead = p.CanRead,
                    CanWrite = p.CanWrite,
                    CanDelete = p.CanDelete
                }).ToList();
            }

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
                var errors = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Json(new { success = false, message = errors });
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

            string result;
            bool isEditMode = isEdit || model.IsEdit;

            if (isEditMode)
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

            if (string.Equals(result, "SUCCESS", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(result, "true", StringComparison.OrdinalIgnoreCase) ||
                (result != null && result.Contains("successfully", StringComparison.OrdinalIgnoreCase)))
            {
                string successMsg = isEditMode ? "Role updated successfully." : "Role registered successfully.";
                return Json(new { success = true, message = successMsg });
            }

            return Json(new { success = false, message = result });
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

        private List<MenuPermissionViewModel> SortMenuHierarchy(List<MenuPermissionViewModel> rawList)
        {
            if (rawList == null || !rawList.Any())
                return new List<MenuPermissionViewModel>();

            var sortedList = new List<MenuPermissionViewModel>();

            
            var rootMenus = rawList
                .Where(m => !m.ParentId.HasValue || m.ParentId == 0)
                .ToList();

            
            foreach (var root in rootMenus)
            {
                AddMenuAndChildren(root, rawList, sortedList);
            }

           
            var addedIds = sortedList.Select(x => x.MenuId).ToHashSet();
            var orphanMenus = rawList.Where(m => !addedIds.Contains(m.MenuId)).ToList();
            sortedList.AddRange(orphanMenus);

            return sortedList;
        }

        private void AddMenuAndChildren(MenuPermissionViewModel parent, List<MenuPermissionViewModel> allMenus, List<MenuPermissionViewModel> result)
        {
            result.Add(parent);

            var children = allMenus
                .Where(m => m.ParentId.HasValue && m.ParentId.Value == parent.MenuId)
                .ToList();

            foreach (var child in children)
            {
                AddMenuAndChildren(child, allMenus, result);
            }
        }
    }
}
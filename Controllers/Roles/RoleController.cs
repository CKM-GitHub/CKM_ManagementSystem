using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Roles;
using CKM_ManagementSystem.Models.ViewModels;

namespace CKM_ManagementSystem.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleBL _roleBL;

        public RoleController(RoleBL roleBL)
        {
            _roleBL = roleBL;
        }

        #region Role List Action Methods

        [HttpGet]
        public async Task<IActionResult> RoleList(int pageNumber = 1, int pageSize = 10, string searchKeyword = "", int? status = null)
        {
            try
            {
                var pagedResult = await _roleBL.GetRoleListPagedAsync(pageNumber, pageSize, searchKeyword, status);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(new RoleListPagedViewModel());
            }
        }

        #endregion

        #region Role Entry & Edit Actions

        [HttpGet]
        public async Task<IActionResult> RoleEntry(string? code)
        {
            bool isEdit = !string.IsNullOrEmpty(code);
            RoleEntryViewModel model;

            if (isEdit)
            {
                var existingRole = await _roleBL.GetRoleByCodeAsync(code!);

                if (existingRole != null)
                {
                    model = existingRole;
                }
                else
                {
                    model = new RoleEntryViewModel();
                }

                var permissions = await _roleBL.GetMenuPermissionsAsync(code);
                model.MenuPermissions = MapToRolePermissions(permissions);
            }
            else
            {
                model = new RoleEntryViewModel
                {
                    MenuPermissions = new List<RolePermissionViewModel>()
                };

                var permissions = await _roleBL.GetMenuPermissionsAsync(null);
                model.MenuPermissions = MapToRolePermissions(permissions);
            }

            model.MenuPermissions = SortMenuHierarchy(model.MenuPermissions);
            ViewBag.IsEdit = isEdit;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CheckDuplicateCode(string roleCode)
        {
            if (string.IsNullOrWhiteSpace(roleCode))
            {
                return Json(new { isDuplicate = false });
            }

            bool isDuplicate = await _roleBL.CheckDuplicateRoleCodeSPAsync(roleCode);
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

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleCode)
        {
            if (string.IsNullOrWhiteSpace(roleCode))
            {
                return Json(new { success = false, message = "Invalid Role Code." });
            }

            var result = await _roleBL.DeleteRoleAsync(roleCode);
            return Json(new { success = result.Success, message = result.Message });
        }

        #endregion

        #region Helper Methods

        private List<RolePermissionViewModel> MapToRolePermissions(List<MenuPermissionViewModel> srcList)
        {
            var list = new List<RolePermissionViewModel>();
            if (srcList != null)
            {
                foreach (var item in srcList)
                {
                    list.Add(new RolePermissionViewModel
                    {
                        MenuId = item.MenuId,
                        MenuName = item.MenuName,
                        ParentId = item.ParentId,
                        CanRead = item.CanRead,
                        CanWrite = item.CanWrite,
                        CanDelete = item.CanDelete
                    });
                }
            }
            return list;
        }

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

        #endregion
    }
}
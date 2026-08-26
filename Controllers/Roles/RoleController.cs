using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.Models.ViewModels.Roles;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> SaveRole(RoleEntryViewModel model, bool isEdit = false)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .FirstOrDefault();
                return Json(new { success = false, message = errors ?? "Validation failed." });
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
                    if (await _roleBL.CheckDuplicateRoleCodeSPAsync(model.RoleCode))
                    {
                        return Json(new { success = false, message = "This Role Code already exists." });
                    }
                    result = _roleBL.Role_Insert(role, permissions);
                }

                if (result == "true" || result == "1")
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

        private static List<RolePermissionViewModel> SortMenuHierarchy(List<RolePermissionViewModel> rawList)
        {
            if (rawList == null || !rawList.Any())
                return new List<RolePermissionViewModel>();

            var sortedList = new List<RolePermissionViewModel>();

            var mainMenus = rawList.Where(m => m.ParentId == null || m.ParentId == 0)
                                   .OrderBy(m => m.MenuId)
                                   .ToList();

            foreach (var main in mainMenus)
            {
                sortedList.Add(main);

                var subMenus = rawList.Where(m => m.ParentId == main.MenuId)
                                      .OrderBy(m => m.MenuId)
                                      .ToList();
                sortedList.AddRange(subMenus);
            }

            var orphanMenus = rawList.Except(sortedList).ToList();
            if (orphanMenus.Any())
            {
                sortedList.AddRange(orphanMenus.OrderBy(m => m.MenuId));
            }

            return sortedList;
        }
    }
}
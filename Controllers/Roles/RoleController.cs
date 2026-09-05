using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Roles;
using CKM_ManagementSystem.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        public IActionResult RoleEntry(string code)
        {
            var model = new RoleEntryViewModel();

            if (!string.IsNullOrEmpty(code))
            {
                var existingRole = _roleBL.GetRoleByCodeViewModel(code);
                if (existingRole != null)
                {
                    model = existingRole;
                    model.IsEdit = true;
                }
                model.MenuPermissions = _roleBL.GetMenuPermissions(code);
            }
            else
            {
                model.MenuPermissions = _roleBL.GetMenuPermissions(null);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRole(RoleEntryViewModel model)
        {
            
            if (!string.IsNullOrWhiteSpace(model.RoleCode) && !Regex.IsMatch(model.RoleCode, @"^[a-zA-Z0-9_-]+$"))
            {
                ModelState.AddModelError("RoleCode", "Role Code contains invalid characters. Only alphanumeric, underscore and hyphen are allowed.");
            }

           
            if (!string.IsNullOrWhiteSpace(model.DisplayName) && Regex.IsMatch(model.DisplayName, @"[<>'""&;]"))
            {
                ModelState.AddModelError("DisplayName", "Role Name contains invalid special characters.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                );
                return Json(new { success = false, errors = errors });
            }

            try
            {
                if (!model.IsEdit && _roleBL.IsRoleCodeDuplicate(model.RoleCode))
                {
                    return Json(new { success = false, message = "Role Code already exists in the system." });
                }

                var roleEntity = new Roles
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

                if (model.IsEdit)
                {
                    _roleBL.Role_Update(roleEntity, permissions);

                    TempData["SuccessMessage"] = "Update is complete.";
                    return Json(new
                    {
                        success = true,
                        isEdit = true,
                        redirectUrl = Url.Action("RoleList", "Role")
                    });
                }
                else
                {
                    _roleBL.Role_Insert(roleEntity, permissions);

                    return Json(new
                    {
                        success = true,
                        isEdit = false,
                        message = "Registration is complete."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while saving: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult RoleList(string searchKeyword, int? status, int pageNumber = 1, int pageSize = 10)
        {
            var model = _roleBL.GetRoleListPaged(pageNumber, pageSize, searchKeyword, status);
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteRole(string roleCode, string id)
        {
            string targetCode = !string.IsNullOrWhiteSpace(roleCode) ? roleCode : id;

            if (string.IsNullOrWhiteSpace(targetCode))
            {
                return Json(new { success = false, message = "Invalid Role Code." });
            }

            var result = _roleBL.DeleteRole(targetCode);
            return Json(new { success = result.Success, message = result.Message });
        }
    }
}
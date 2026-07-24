using CKM_ManagementSystem.Models.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using CKM_ManagementSystem.Models;

namespace CKM_ManagementSystem.Controllers
{
    public class RoleController : Controller
    {
        private readonly string _connectionString;

        public RoleController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }


        [HttpGet]
        public IActionResult RoleEntry()
        {
            var model = new RoleEntryViewModel
            {
                MenuPermissions = new List<MenuPermissionViewModel>() 
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CheckDuplicateCode(string roleCode)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var count = await db.ExecuteScalarAsync<int>(
                    "sp_CheckDuplicateRoleCode",
                    new { RoleCode = roleCode },
                    commandType: CommandType.StoredProcedure
                );

                return Json(new { isDuplicate = count > 0 });
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> SaveRole(RoleEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("RoleEntry", model);
            }

            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    await db.ExecuteAsync("sp_SaveRoleInfo", new
                    {
                        RoleCode = model.RoleCode,
                        RoleName = model.DisplayName,
                        Description = model.Description,
                        Status = model.Status
                    }, commandType: CommandType.StoredProcedure);

                    if (model.MenuPermissions != null)
                    {
                        foreach (var perm in model.MenuPermissions)
                        {
                            if (perm.CanRead || perm.CanWrite || perm.CanDelete)
                            {
                                await db.ExecuteAsync("sp_SaveRolePermission", new
                                {
                                    RoleCode = model.RoleCode,
                                    MenuId = perm.MenuId,
                                    CanRead = perm.CanRead,
                                    CanWrite = perm.CanWrite,
                                    CanDelete = perm.CanDelete
                                }, commandType: CommandType.StoredProcedure);
                            }
                        }
                    }
                }

                TempData["SuccessMessage"] = "Role ကို အောင်မြင်စွာ သိမ်းဆည်းပြီးပါပြီ။";
                return RedirectToAction("RoleEntry");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Data သိမ်းဆည်းရာတွင် အမှားအယွင်း ရှိနေပါသည်: " + ex.Message);
                return View("RoleEntry", model);
            }
        }
    }
}
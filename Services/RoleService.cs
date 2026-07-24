using System.Data;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.Services.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CKM_ManagementSystem.Services
{
    public class RoleService : IRoleService
    {
        private readonly string _connectionString;

        public RoleService(IConfiguration configuration)
        {
            
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
        }

        
        public async Task<bool> CheckDuplicateRoleCodeAsync(string roleCode)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var count = await db.ExecuteScalarAsync<int>(
                    "sp_CheckDuplicateRoleCode",
                    new { RoleCode = roleCode },
                    commandType: CommandType.StoredProcedure
                );

                return count > 0; 
            }
        }

        
        public async Task SaveRoleWithPermissionsAsync(RoleEntryViewModel model)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                
                if (db.State == ConnectionState.Closed)
                    await ((SqlConnection)db).OpenAsync();

                
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
        }
    }
}
using CKM_ManagementSystem.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CKM_ManagementSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CkmManagementSystemContext _context;

        public UserRepository(CkmManagementSystemContext context)
        {
            _context = context;
        }

        public async Task<int> CreateUserAsync(User user)
        {
            var errorCodeParam = new SqlParameter("@ErrorCode", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            // String matches the exact names inside your CREATE PROCEDURE definition
            var parameters = new[]
            {
                new SqlParameter("@ID", user.Id),
                new SqlParameter("@Staff_Code", user.StaffCode ?? (object)DBNull.Value),
                new SqlParameter("@Name", user.Name ?? (object)DBNull.Value),
                new SqlParameter("@Email", user.Email ?? (object)DBNull.Value),
                new SqlParameter("@Password", user.Password ?? (object)DBNull.Value),
                new SqlParameter("@Image_URL", user.ImageUrl ?? (object)DBNull.Value),
                new SqlParameter("@Role_Code", user.RoleCode ?? (object)DBNull.Value),
                new SqlParameter("@Gender", user.Gender ?? (object)DBNull.Value),
                new SqlParameter("@Department_Code", user.DepartmentCode ?? (object)DBNull.Value),
                new SqlParameter("@Status", user.Status),
                errorCodeParam
            };

            // Explicitly mapping parameters to avoid positional mismatches
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_CreateUser @ID, @Staff_Code, @Name, @Email, @Password, @Image_URL, @Role_Code, @Gender, @Department_Code, @Status, @ErrorCode OUTPUT",
                parameters
            );

            return (int)errorCodeParam.Value;
        }

        public async Task<IEnumerable<Department>> GetActiveDepartmentsAsync()
        {
            return await _context.Departments
                .Where(x => x.Status == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserRole>> GetUserRolesAsync()
        {
            return await _context.UserRoles.ToListAsync();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CKM_ManagementSystem.Models;
using CKM_ManagementSystem.DTOs;

namespace CKM_ManagementSystem.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly CkmManagementSystemContext _context;
        public UserManagementController(CkmManagementSystemContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> UserList()
        {
            var user = await _context.Users
                .Select(u => new UserManagementDto
                {
                    StaffCode = u.StaffCode,
                    Name = u.Name,
                    Email = u.Email,
                    Department_Name = u.DepartmentCodeNavigation.DepartmentName ?? "N/A",
                    Role_Name = u.RoleCodeNavigation.RoleName ?? "N/A",
                    Status = u.Status,
                    imageUrl = u.ImageUrl
                }).ToListAsync();

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> UserUpdate(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UserUpdateDto 
            { 
               StaffCode = user.StaffCode,
               Name = user.Name,
               Email = user.Email,
               Gender = user.Gender,
               DepartmentCode = user.DepartmentCode,
               RoleCode = user.RoleCode,
               Status = user.Status,
            };

            return View(model);
        }

       [HttpPost]
       public async Task<IActionResult> UserUpdate (UserUpdateDto model)
       {
            return RedirectToAction("UserList ","UserManagement");
       }
    }
}

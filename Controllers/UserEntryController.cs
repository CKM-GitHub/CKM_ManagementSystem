using CKM_ManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using AspNetCoreGeneratedDocument;
using Microsoft.EntityFrameworkCore;

namespace CKM_ManagementSystem.Controllers
{
    public class UserEntryController : Controller
    {
        private readonly CkmManagementSystemContext _context;
        private readonly IWebHostEnvironment _environment;

        public UserEntryController(CkmManagementSystemContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        private readonly PasswordHasher<User> _passwordHasher = new();

        [HttpGet]
        public IActionResult UserCreate()
        {
            ViewBag.DepartmentList = new SelectList(
                _context.Departments, 
                "DepartmentCode",
                "DepartmentName");
            ViewBag.UserRoleList = new SelectList(
                _context.UserRoles,
                "RoleCode", 
                "RoleName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserCreate(UserCreateDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.StaffCode == dto.StaffCode))
            {
                ModelState.AddModelError("StaffCode", "StaffCode is already registered.");
            }

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                ModelState.AddModelError("Email", "This Email is already registered.");
            }
            
            if (!ModelState.IsValid)
            {

                ViewBag.DepartmentList = new SelectList(
                    _context.Departments, 
                    "DepartmentCode", 
                    "DepartmentName",
                    dto.DepartmentCode);
                ViewBag.UserRoleList = new SelectList(
                    _context.UserRoles, 
                    "RoleCode",
                    "RoleName",
                    dto.RoleCode);
                return View(dto);
            }

            // Bar lo folder new generate load lae so dop, new environmet change twar yin a shin phay aung lo
            string? imageUrl = null;   

            if (dto.ImageFile != null)
            {
                string imageFolder = Path.Combine
                    (_environment.WebRootPath,
                    "images", 
                    "users");

                if (!Directory.Exists(imageFolder))
                {
                    Directory.CreateDirectory(imageFolder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                string filePath = Path.Combine(imageFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                imageUrl = "/images/users/" + fileName;
            } 

            var user = new User
            {
                Id = Guid.NewGuid(),
                StaffCode = dto.StaffCode,
                Name = dto.Name,
                Email = dto.Email,
                Gender = dto.Gender,
                DepartmentCode = dto.DepartmentCode,
                RoleCode = dto.RoleCode,
                Status = dto.Status,
                ImageUrl = imageUrl
            };

            user.Password = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "User created successfully";

            return RedirectToAction("UserCreate");
        }
    }
}


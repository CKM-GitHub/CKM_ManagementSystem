using CKM_ManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using CKM_ManagementSystem.Repositories;

namespace CKM_ManagementSystem.Controllers
{
    public class UserEntryController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public UserEntryController(IWebHostEnvironment environment, IUserRepository userRepository)
        {
            _environment = environment;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> UserCreate()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserCreate(UserCreateViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(dto.DepartmentCode, dto.RoleCode);
                return View(dto);
            }

            // Bar lo folder new generate load lae so dop, new environmet change twar yin a shin phay aung lo
            string? imageUrl = null;
            if (dto.ImageFile != null)
            {
                string imageFolder = Path.Combine(_environment.WebRootPath, "images", "users");

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

            int errorCode = await _userRepository.CreateUserAsync(user);

            if (errorCode == 1)
            {
                ModelState.AddModelError("StaffCode", "StaffCode is already registered.");
            }
            else if (errorCode == 2)
            {
                ModelState.AddModelError("Email", "This Email is already registered.");
            }
            else if (errorCode == -1)
            {
                ModelState.AddModelError(string.Empty, "An unexpected system error occurred on the database tier.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(dto.DepartmentCode, dto.RoleCode);
                return View(dto);
            }

            TempData["SuccessMessage"] = "User created successfully";
            return RedirectToAction("UserCreate","UserEntry");
        }

        private async Task PopulateDropdownsAsync(string? selectedDept = null, string? selectedRole = null)
        {
            var departments = await _userRepository.GetActiveDepartmentsAsync();
            var roles = await _userRepository.GetUserRolesAsync();

            ViewBag.DepartmentList = new SelectList(departments, "DepartmentCode", "DepartmentName", selectedDept);
            ViewBag.UserRoleList = new SelectList(roles, "RoleCode", "RoleName", selectedRole);
        }
    }
}



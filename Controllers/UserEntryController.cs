using CKM_ManagementSystem.Models;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public async Task<IActionResult> UserCreate(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model.DepartmentCode, model.RoleCode);
                return View(model);
            }

            // Bar lo folder new generate load lae so dop, new environmet change twar yin a shin phay aung lo
            string? imageUrl = null;
            if (model.ImageFile != null)
            {
                string imageFolder = Path.Combine(_environment.WebRootPath, "images", "users");

                if (!Directory.Exists(imageFolder))
                {
                    Directory.CreateDirectory(imageFolder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                string filePath = Path.Combine(imageFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                imageUrl = "/images/users/" + fileName;
            }

            var user = new User
            {
                StaffCode = model.StaffCode,
                Name = model.Name,
                Email = model.Email,
                Gender = model.Gender,
                DepartmentCode = model.DepartmentCode,
                RoleCode = model.RoleCode,
                Status = model.Status,
                ImageUrl = imageUrl
            };

            user.Password = _passwordHasher.HashPassword(user, model.Password);

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
                await PopulateDropdownsAsync(model.DepartmentCode, model.RoleCode);
                return View(model);
            }

            TempData["SuccessMessage"] = "User created successfully";
            return RedirectToAction("UserCreate", "UserEntry");
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



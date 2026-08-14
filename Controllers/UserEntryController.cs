using CKM_ManagementSystem.BL.Interface;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CKM_ManagementSystem.Controllers
{
    public class UserEntryController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IUserEntryBL _userEntryBL;

        public UserEntryController(
            IWebHostEnvironment environment,
            IUserEntryBL userEntryBL)
        {
            _environment = environment;
            _userEntryBL = userEntryBL;
        }

        [HttpGet]
        public async Task<IActionResult> UserCreate()
        {
            var model = new UserCreateViewModel();

            await PopulateDropdownsAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserCreate(UserCreateViewModel model)
        {
            Console.WriteLine("=== POST START ===");
            Console.WriteLine($"ImageFile: {model.ImageFile?.FileName}");
            Console.WriteLine($"TempImageName: {model.TempImageName}");
            Console.WriteLine($"ImageUrl: {model.ImageUrl}");

            // 1. New image uploaded
            if (model.ImageFile != null)
            {
                string tempFolder = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "temp"
                );

                Directory.CreateDirectory(tempFolder);

                string tempFileName =
                    Guid.NewGuid()
                    + Path.GetExtension(model.ImageFile.FileName);

                string tempPath = Path.Combine(
                    tempFolder,
                    tempFileName
                );

                using (var stream = new FileStream(
                    tempPath,
                    FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                model.TempImageName = tempFileName;

                // Important:
                // The browser submitted an empty TempImageName,
                // so remove the old ModelState value.
                ModelState.Remove(nameof(model.TempImageName));
            }

            // 2. ImageUrl is based on existing temp image
            if (!string.IsNullOrEmpty(model.TempImageName))
            {
                model.ImageUrl =
                    "/images/users/" + model.TempImageName;

                ModelState.Remove(nameof(model.ImageUrl));
            }

            // 3. Validation
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(
                    model.DepartmentCode,
                    model.RoleCode
                );

                return View(model);
            }

            // 4. Create User
            int errorCode =
                await _userEntryBL.CreateUserAsync(model);

            // 5. DB errors
            if (errorCode == 1)
            {
                ModelState.AddModelError(
                    "StaffCode",
                    "StaffCode is already registered."
                );
            }
            else if (errorCode == 2)
            {
                ModelState.AddModelError(
                    "Email",
                    "This Email is already registered."
                );
            }
            else if (errorCode == 3)
            {
                ModelState.AddModelError(
                    "DepartmentCode",
                    "Selected department does not exist."
                );
            }
            else if (errorCode == 4)
            {
                ModelState.AddModelError(
                    "RoleCode",
                    "Selected role does not exist."
                );
            }
            else if (errorCode == -1)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "An unexpected system error occurred on the database tier."
                );
            }

            // 6. DB failed → KEEP TEMP IMAGE
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(
                    model.DepartmentCode,
                    model.RoleCode
                );

                return View(model);
            }

            // 7. DB success → TEMP → USERS
            if (!string.IsNullOrEmpty(model.TempImageName))
            {
                string tempFolder = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "temp"
                );

                string userFolder = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "users"
                );

                Directory.CreateDirectory(userFolder);

                string tempPath = Path.Combine(
                    tempFolder,
                    model.TempImageName
                );

                string finalPath = Path.Combine(
                    userFolder,
                    model.TempImageName
                );

                if (System.IO.File.Exists(tempPath))
                {
                    System.IO.File.Move(
                        tempPath,
                        finalPath
                    );
                }
            }

            TempData["SuccessMessage"] =
                "User created successfully";

            return RedirectToAction(
                "UserCreate",
                "UserEntry"
            );
        }

        private async Task PopulateDropdownsAsync(
            string? selectedDept = null,
            string? selectedRole = null)
        {
            var departments =
                await _userEntryBL.GetDepartmentsAsync();

            var roles =
                await _userEntryBL.GetUserRolesAsync();

            ViewBag.DepartmentList =
                new SelectList(
                    departments,
                    "DepartmentCode",
                    "DepartmentName",
                    selectedDept
                );

            ViewBag.UserRoleList =
                new SelectList(
                    roles,
                    "RoleCode",
                    "RoleName",
                    selectedRole
                );
        }
    }
}
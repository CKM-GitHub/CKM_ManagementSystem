using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CKM_ManagementSystem.Controllers
{
    public class UserCreateController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly UserEntryBL _userEntryBL;

        public UserCreateController(
            IWebHostEnvironment environment,
            UserEntryBL userEntryBL)
        {
            _environment = environment;
            _userEntryBL = userEntryBL;
        }

        [HttpGet]
        public async Task<IActionResult> UserCreate(string? source)
        {
            ViewBag.Source = source;

            var model = new UserCreateViewModel();

            await PopulateDropdownsAsync();

            return View("~/Views/UserList/UserCreate.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCreate(UserCreateViewModel model, string? source)
        {
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

                ModelState.Remove(nameof(model.TempImageName));
            }
            if (!string.IsNullOrEmpty(model.TempImageName))
            {
                model.ImageUrl =
                    "/images/users/" + model.TempImageName;

                ModelState.Remove(nameof(model.ImageUrl));
            }
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(
                    model.DepartmentCode,
                    model.RoleCode
                );

                return View("~/Views/UserList/UserCreate.cshtml", model);
            }

            int errorCode =
                await _userEntryBL.CreateUserAsync(model);

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

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(
                    model.DepartmentCode,
                    model.RoleCode
                );

                return View("~/Views/UserList/UserCreate.cshtml", model);
            }

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
                "UserCreate",
                new { source = source }
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
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.BL.Interface;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.InteropServices;

namespace CKM_ManagementSystem.Controllers
{
    public class UserUpdateController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IUserUpdateBL _userUpdateBL;
        public UserUpdateController(IWebHostEnvironment environment, IUserUpdateBL userUpdateBL)
        {
            _userUpdateBL = userUpdateBL;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> UserUpdate(string StaffCode)
        {
            if (string.IsNullOrWhiteSpace(StaffCode))
            {
                TempData["ErrorMessage"] = "Fail";
                return RedirectToAction("UserList", "UserList");
            }
            var model = await _userUpdateBL.GetUserByStaffCodeAsync(StaffCode);

            if (model == null)
            {
                TempData["ErrorMessage"] = "User Not Found";
                return RedirectToAction("UserList", "UserList");
            }
            var departments = await _userUpdateBL.GetDepartmentsAsync();
            var roles = await _userUpdateBL.GetRolesAsync();

            ViewBag.DepartmentList = new SelectList(
            departments,
            "DepartmentCode",
            "DepartmentName",
            model.DepartmentCode);

            ViewBag.UserRoleList = new SelectList(
             roles,
             "RoleCode",
             "RoleName",
              model.RoleCode);
            return View("~/Views/UserList/UserUpdate.cshtml", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserUpdate(UserUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await loadDropdownAsync(model);
                return View("~/Views/UserList/UserUpdate.cshtml", model);
            }

            if (model.ImageFile != null)
            {
                model.ImageUrl = await SaveImageAsync(model.ImageFile);
            }

            int errorCode = await _userUpdateBL.UserUpdateAsync(model);

            if (errorCode == 0)
            {
                TempData["SuccessMessage"] = "User Update Successfully";
                return RedirectToAction("UserList", "UserList");
            }

            if (errorCode == 2)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "This Email already exists."
                );
            }
            else if (errorCode == 3)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "User not found."
                );
            }
            else
            {
                ModelState.AddModelError(
                    string.Empty,
                    "User update failed."
                );
            }

            await loadDropdownAsync(model);

            return View("~/Views/UserList/UserUpdate.cshtml", model);
        }
        private async Task loadDropdownAsync(UserUpdateViewModel model)
        {
            var departments = await _userUpdateBL.GetDepartmentsAsync();
            var roles = await _userUpdateBL.GetRolesAsync();

            ViewBag.DepartmentList = new SelectList(
            departments,
            "DepartmentCode",
            "DepartmentName",
            model.DepartmentCode);

            ViewBag.UserRoleList = new SelectList(
             roles,
             "RoleCode",
             "RoleName",
              model.RoleCode);
        }
    private async Task<string?> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null)
            {
                return null;
            }

            string imageFolder = Path.Combine(
                _environment.WebRootPath,
                "images",
                "users");

            Directory.CreateDirectory(imageFolder);

            string fileName =
                $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";

            string filePath = Path.Combine(imageFolder, fileName);

            using var stream = new FileStream(
                filePath,
                FileMode.Create);

            await imageFile.CopyToAsync(stream);

            return $"/images/users/{fileName}";
        }
    }
}
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Reflection.Metadata.Ecma335;

namespace CKM_ManagementSystem.Controllers.User
{
    public class UserListController : Controller
    {
        private readonly UserListBL _userListBL;
        private readonly IWebHostEnvironment _environment;

        public UserListController(UserListBL userListBL, IWebHostEnvironment environment)
        {
            _userListBL = userListBL;
            _environment = environment;
        }
        public async Task<IActionResult> UserList (
            string? searchText,
            bool? status,
            string? departmentCode,
            string? roleCode,
            int pageNumber = 1,
            int pageSize = 10)
        {
            ViewBag.SearchText = searchText;
            ViewBag.Status = status;
            ViewBag.DepartmentCode = departmentCode;
            ViewBag.RoleCode = roleCode;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            var model = await _userListBL.GetUserListAsync(
                searchText,
                status,
                departmentCode,
                roleCode,
                pageNumber,
                pageSize
                );

            if (model.ErrorCode != 0)
            {
                ModelState.AddModelError(string.Empty, "Error Shi Dl !!");
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string staffCode)
        {
            if (string.IsNullOrWhiteSpace(staffCode))
            {
                TempData["ErrorMessage"] = "Sraff Code is Required!!";

                return RedirectToAction("UserList");
            }
            
            var result = await _userListBL.DeleteUserAsync(staffCode);
            if (result.ErrorCode == 0)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                   TempData["ErrorMessage"] = "User could not be deleted";
            }

            return RedirectToAction("UserList");
        }
        [HttpGet]
        public async Task<IActionResult> UserUpdate(string StaffCode)
        {
            CleanUpTempImages();

            if (string.IsNullOrWhiteSpace(StaffCode))
            {
                TempData["ErrorMessage"] = "Fail";
                return RedirectToAction("UserList", "UserList");
            }
            var model = await _userListBL.GetUserByStaffCodeAsync(StaffCode);

            if (model == null)
            {
                TempData["ErrorMessage"] = "User Not Found";
                return RedirectToAction("UserList", "UserList");
            }
            var departments = await _userListBL.GetDepartmentsAsync();
            var roles = await _userListBL.GetRolesAsync();

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

            model.Mode = "Update";
            return View("~/Views/UserList/UserCreate.cshtml", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserUpdate(UserCreateViewModel model)
        {
            model.Mode = "Update";

            if (model.ImageFile != null)
            {
                string tempFolder = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "temp");

                Directory.CreateDirectory(tempFolder);

                string tempFileName =
                    $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";

                string tempFilePath = Path.Combine(
                    tempFolder,
                    tempFileName);

                await using var stream = new FileStream(
                    tempFilePath,
                    FileMode.Create);

                await model.ImageFile.CopyToAsync(stream);

                model.TempImageName = tempFileName;

                model.ImageUrl = $"/images/users/{tempFileName}";

                ModelState.Remove(nameof(model.ImageFile));
                ModelState.Remove(nameof(model.ImageUrl));
            }

            if (!ModelState.IsValid)
            {
                await loadDropdownAsync(model);

                return View("~/Views/UserList/UserCreate.cshtml", model);
            }

            int errorCode = await _userListBL.UserUpdateAsync(model);        

            if (errorCode == 0)
            {
                if (!string.IsNullOrEmpty(model.TempImageName))
                {
                    string tempFolder = Path.Combine(_environment.WebRootPath, "images", "temp");
                    string userFolder = Path.Combine(_environment.WebRootPath, "images", "users");
                    Directory.CreateDirectory(userFolder);

                    string tempFilePath = Path.Combine(tempFolder, model.TempImageName);
                    string finalPath = Path.Combine(userFolder, model.TempImageName);
                    if (System.IO.File.Exists(tempFilePath))
                    {
                        System.IO.File.Move(tempFilePath, finalPath);
                    }
                }
                TempData["SuccessMessage"] = "User Update Successfully";
                return RedirectToAction("UserCreate","UserCreate", new {mode = "Update", staffCode = model.StaffCode });
            }

            if (errorCode == 2)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "This Email already exists.");
            }
            else if (errorCode == 3)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "User not found.");
            }
            else
            {
                ModelState.AddModelError(
                    string.Empty,
                    "User update failed.");
            }

            await loadDropdownAsync(model);

            return View("~/Views/UserList/UserCreate.cshtml", model);
        }
        private async Task loadDropdownAsync(UserCreateViewModel model)
        {
            var departments = await _userListBL.GetDepartmentsAsync();
            var roles = await _userListBL.GetRolesAsync();

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
        /*
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
        } */
        private void CleanUpTempImages()
        {
            string tempFolder = Path.Combine(
                _environment.WebRootPath,
                "images",
                "temp"
                );

            if (!Directory.Exists(tempFolder))
                return;

            DateTime expireTime = DateTime.UtcNow.AddMinutes(-30);

            foreach (string file in Directory.GetFiles(tempFolder))
            { 
                if(System.IO.File.GetLastWriteTime(file) < expireTime)
                {
                    System.IO.File.Delete(file);
                }
            }
        }
    }    
}

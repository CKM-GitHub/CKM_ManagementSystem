using CKM_ManagementSystem.Authorization;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels.User;
using CKM_ManagementSystem.Permissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace CKM_ManagementSystem.Controllers.User
{
    public class UserListController : Controller
    {
        private readonly UserListBL _userListBL;
        private readonly IWebHostEnvironment _environment;
        private readonly CurrentUserPermission _permission;   

        public UserListController(
            UserListBL userListBL,
            IWebHostEnvironment environment,
            CurrentUserPermission permission)                  
        {
            _userListBL = userListBL;
            _environment = environment;
            _permission = permission;                         
        }

        [Authorize(Policy = "Permission.User.Read")]
        public async Task<IActionResult> UserList(
            string? searchText,
            bool? status,
            string? departmentCode,
            string? roleCode,
            int pageNumber = 1,
            int pageSize = 6)
        {
            ViewBag.SearchText = searchText;
            ViewBag.Status = status;
            ViewBag.DepartmentCode = departmentCode;
            ViewBag.RoleCode = roleCode;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            var model = await _userListBL.GetUserListAsync(
                searchText, status, departmentCode, roleCode,
                pageNumber, pageSize);

            if (model.ErrorCode != 0)
            {
                ModelState.AddModelError(string.Empty, "Error Shi Dl !!");
            }

            model.CanWrite = _permission.CanWrite(MenuIDs.User);
            model.CanDelete = _permission.CanDelete(MenuIDs.User);
            // ─────────────

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "Permission.User.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string staffCode)
        {
            if (string.IsNullOrWhiteSpace(staffCode))
            {
                TempData["ErrorMessage"] = "Staff Code is Required!!";
                return RedirectToAction("UserList");
            }

            var result = await _userListBL.DeleteUserAsync(staffCode);

            if (result.ErrorCode == 0)
                TempData["SuccessMessage"] = "User deleted successfully.";
            else
                TempData["ErrorMessage"] = "User could not be deleted";

            return RedirectToAction("UserList");
        }

        [HttpGet]
        [Authorize(Policy = "Permission.User.Write")]   // ← Read → Write ပြင်
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
                departments, "DepartmentCode", "DepartmentName",
                model.DepartmentCode);

            ViewBag.UserRoleList = new SelectList(
                roles, "RoleCode", "RoleName", model.RoleCode);

            // ── ★ ဒါ ထည့် ──
            model.CanWrite = _permission.CanWrite(MenuIDs.User);
            // ─────────────

            model.Mode = "Update";
            return View("~/Views/UserList/UserCreate.cshtml", model);
        }

        [HttpPost]
        [Authorize(Policy = "Permission.User.Write")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserUpdate(UserCreateViewModel model)
        {
            model.Mode = "Update";

            if (model.ImageFile != null)
            {
                string tempFolder = Path.Combine(
                    _environment.WebRootPath, "images", "temp");

                Directory.CreateDirectory(tempFolder);

                string tempFileName =
                    $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";

                string tempFilePath = Path.Combine(tempFolder, tempFileName);

                await using var stream = new FileStream(tempFilePath, FileMode.Create);
                await model.ImageFile.CopyToAsync(stream);

                model.TempImageName = tempFileName;
                model.ImageUrl = $"/images/users/{tempFileName}";

                ModelState.Remove(nameof(model.ImageFile));
                ModelState.Remove(nameof(model.ImageUrl));
            }

            // ── ★ CanWrite ကို အရင် သတ်မှတ် (error ပြန်လည်းရ ရှိစေ) ──
            model.CanWrite = _permission.CanWrite(MenuIDs.User);
            // ──────────────────────────────────

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
                return RedirectToAction("UserCreate", "UserCreate",
                    new { mode = "Update", staffCode = model.StaffCode });
            }

            if (errorCode == 2)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "This Email already exists.");
            }
            else if (errorCode == 3)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User update failed.");
            }

            await loadDropdownAsync(model);
            return View("~/Views/UserList/UserCreate.cshtml", model);
        }

        private async Task loadDropdownAsync(UserCreateViewModel model)
        {
            var departments = await _userListBL.GetDepartmentsAsync();
            var roles = await _userListBL.GetRolesAsync();

            ViewBag.DepartmentList = new SelectList(
                departments, "DepartmentCode", "DepartmentName",
                model.DepartmentCode);

            ViewBag.UserRoleList = new SelectList(
                roles, "RoleCode", "RoleName", model.RoleCode);
        }

        private void CleanUpTempImages()
        {
            string tempFolder = Path.Combine(
                _environment.WebRootPath, "images", "temp");

            if (!Directory.Exists(tempFolder))
                return;

            DateTime expireTime = DateTime.UtcNow.AddMinutes(-30);

            foreach (string file in Directory.GetFiles(tempFolder))
            {
                if (System.IO.File.GetLastWriteTime(file) < expireTime)
                {
                    System.IO.File.Delete(file);
                }
            }
        }
    }
}
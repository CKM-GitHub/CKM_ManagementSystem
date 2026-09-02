using CKM_ManagementSystem.DL;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CKM_ManagementSystem.Models.ViewModels;

namespace CKM_ManagementSystem.MenuBL
{
    public class Menu_BL : BaseDL
    {
        public Menu_BL(IConfiguration configuration)
            :base(configuration){ }

        public async Task<MenuActionResult> CreateMenuAsync(
            string? menuName,
            string? actionName,
            string? controllerName,
            string? menuIcon,
            int? displayOrder,
            int? parentMenuId,
            bool isSubMenu,
            bool status)
        {
            var validationError = ValidateMenuInput(menuName, displayOrder, isSubMenu, parentMenuId);
            if (validationError != null) return validationError;

            int? actualParentId = (parentMenuId.HasValue && parentMenuId.Value > 0) ? parentMenuId.Value : null;
            bool calculatedIsSubMenu = actualParentId.HasValue;

            var (statusCodeParam, statusMessageParam) = CreateOutputParameters();
            
            var parameters = new[]
            {
                new SqlParameter ("@MenuName", (object?)menuName ?? DBNull.Value),
                new SqlParameter ("@ActionName", (object?)actionName ?? DBNull.Value),
                new SqlParameter ("@ControllerName",(object?)controllerName ?? DBNull.Value),
                new SqlParameter ("@MenuIcon", string.IsNullOrWhiteSpace(menuIcon) ? DBNull.Value : menuIcon),
                new SqlParameter ("@DisplayOrder", (object?)displayOrder ?? DBNull.Value),
                new SqlParameter ("@ParentMenuId", (object?)parentMenuId ?? DBNull.Value),
                new SqlParameter("@IsSubMenu", calculatedIsSubMenu),
                new SqlParameter("@Status", status),
                statusCodeParam,
                statusMessageParam
            };
            await ExecuteAsync("sp_CreateMenu", parameters);

            return ParseActionResult(statusCodeParam, statusMessageParam);
        }

        public async Task<MenuActionResult> UpdateMenuAsync(
            int menuId,
            string? menuName,
            string? actionName,
            string? controllerName,
            string? menuIcon,
            int? displayOrder,
            int? parentMenuId,
            bool isSubMenu,
            bool status)
        {
            if (menuId <= 0)
            {
               return InvalidResult("Invalid Menu ID");
            }
            var validationError = ValidateMenuInput(menuName, displayOrder, isSubMenu, parentMenuId);
            if (validationError != null) return validationError;

            int? actualParentId = (parentMenuId.HasValue && parentMenuId.Value > 0) ? parentMenuId.Value : null;
            var (statusCodeParam, statusMessageParam) = CreateOutputParameters();

            var parameters = new[]
            {
                new SqlParameter("@MenuID", menuId),
                new SqlParameter("@MenuName", (object?)menuName ?? DBNull.Value),
                new SqlParameter ("@ActionName", (object?)actionName ?? DBNull.Value),
                new SqlParameter ("@ControllerName", (object?)controllerName ?? DBNull.Value),
                new SqlParameter("@MenuIcon", string.IsNullOrWhiteSpace(menuIcon) ? DBNull.Value : menuIcon),
                new SqlParameter("@DisplayOrder", (object?)displayOrder ?? DBNull.Value),
                new SqlParameter("@ParentMenuId", (object?)parentMenuId ?? DBNull.Value),
                new SqlParameter("@Status", status),
                statusCodeParam,
                statusMessageParam
            };
            await ExecuteAsync("sp_Menu_Update", parameters);

            return ParseActionResult(statusCodeParam, statusMessageParam);
        }
        public async Task<MenuListItem?> GetMenuByIdAsync(int menuId)
        {
            if (menuId <= 0) return null;
            var parameters = new[]
            {
                new SqlParameter("@MenuID", menuId)
            };
            DataTable dt = await SelectDataTableAsync("sp_Menu_GetById", parameters);
            if (dt == null || dt.Rows.Count == 0) return null;

            return MapDataRowToMenuListItem(dt.Rows[0], dt.Columns);
        }
        public async Task<MenuListViewModel> GetPagedMenuListAsync(string? searchTerm, int? parentMenuId, bool? statusFilters = null, int page = 1, int pageSize = 10)
        {
            var totalCountParam = new SqlParameter
            {
                ParameterName = "@TotalCount",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };
            var parameters = new[]
            {
                new SqlParameter("@SearchTerm", string.IsNullOrWhiteSpace(searchTerm) ? DBNull.Value : searchTerm),
                new SqlParameter("@ParentMenuId", (object?)parentMenuId ?? DBNull.Value),
                new SqlParameter("@StatusFilters", (object?)statusFilters ?? DBNull.Value),
                new SqlParameter("@PageNumber", page),
                new SqlParameter("@PageSize", pageSize),
                totalCountParam
            };
            DataTable dt = await SelectDataTableAsync("sp_GetMenuList", parameters);
            var menuList = dt.Rows.Cast<DataRow>()
                    .Select(row => MapDataRowToMenuListItem(row, dt.Columns))
                    .ToList();
            int totalItems = (totalCountParam.Value != DBNull.Value) ? Convert.ToInt32(totalCountParam.Value) : 0;
            return new MenuListViewModel
            {
                SearchTerm = searchTerm,
                SelectedParentId = parentMenuId,
                StatusFilters = statusFilters,
                Menus = menuList,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / (pageSize > 0 ? pageSize:10)),
                TotalItems = totalItems,
                PageSize = pageSize,
            };
        }
        public async Task<MenuActionResult> DeleteMenuAsync(int menuId)
        {
            if(menuId <= 0)
            {
                return InvalidResult("Invalid Menu ID.");
            }

            var (statusCodeParam, statusMessageParam) = CreateOutputParameters();

            var parameters = new[]
            {
                new SqlParameter ("@MenuID", menuId),
                statusCodeParam,
                statusMessageParam
            };

            await ExecuteAsync("sp_DeleteMenu", parameters);

            return ParseActionResult(statusCodeParam, statusMessageParam);
        }

        public async Task<List<SelectListItem>> GetParentMenusForDropdownAsync()
        {
            DataTable table = await SelectDataTableAsync("sp_GetParentMenusDropdown");
            var selectList = new List<SelectListItem>();
            foreach(DataRow row in table.Rows)
            {
                selectList.Add(new SelectListItem
                {
                    Value = row["MenuID"].ToString(),
                    Text = row["MenuName"].ToString()
                });
            }
            return selectList;
        }

        public async Task<List<SelectListItem>> GetParentMenuListAsync()
        {
            //var allMenus = await GetMenuListAsync(searchTerm: null, parentMenuId: null, statusFilters: true);
            //var parentMenus = allMenus
              //  .Where(m => m.ParentMenuId == null || m.ParentMenuId == 0) 
                //.Select(m => new SelectListItem
                //{
                  //  Value = m.MenuID.ToString(),
                    //Text = m.MenuName,
                //})
                //.ToList();
            var parentMenus = await GetParentMenusForDropdownAsync();
            parentMenus.Insert(0, new SelectListItem
            {
                Value = "-1",
                Text = "Main Menu"
            });
            return parentMenus;
        }

       private MenuListItem MapDataRowToMenuListItem(DataRow row, DataColumnCollection columns)
        {
            string controllerName = row["ControllerName"] != DBNull.Value ? row["ControllerName"].ToString()! : string.Empty;
            string actionName = row["ActionName"] != DBNull.Value ? row["ActionName"].ToString()! : string.Empty;

            return new MenuListItem
            {
                MenuID = Convert.ToInt32(row["MenuID"]),
                MenuName = row["MenuName"] != DBNull.Value ? row["MenuName"].ToString()! : string.Empty,
                IconClass = row["MenuIcon"] != DBNull.Value ? row["MenuIcon"].ToString() : null,

                ControllerName = controllerName,
                ActionName = actionName,
                Route = !string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName)
                         ? $"/{controllerName}/{actionName}".ToLower()
                         : "#",
                ParentMenuId = columns.Contains("ParentMenuId") && row["ParentMenuId"] != DBNull.Value
                                ? Convert.ToInt32(row["ParentMenuId"])
                                : null,
                ParentMenuName = row["ParentMenuName"] != DBNull.Value ? row["ParentMenuName"].ToString()! : "Main Menu",
                DisplayOrder = row["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(row["DisplayOrder"]) : 0,
                Status = row["Status"] != DBNull.Value && Convert.ToBoolean(row["Status"])
            };
        }
        private MenuActionResult? ValidateMenuInput(string? menuName, int? displayOrder, bool isSubMenu, int? parentMenuId)
        {
            if(string.IsNullOrWhiteSpace(menuName)) return InvalidResult("Menu Name is required.");

            if (isSubMenu && (!parentMenuId.HasValue || parentMenuId.Value <= 0)) 
                return InvalidResult("Please select a Parent Menu.");

            if (!displayOrder.HasValue || displayOrder.Value < 0 || displayOrder.Value > 999)
                return InvalidResult("Display Order must be a 3-digit number (e.g., 000 to 999).");
            return null;
        }
        private MenuActionResult InvalidResult(string message)
        {
            return new MenuActionResult
            {
                StatusCode = -1,
                StatusMessage = message
            };
        }
        private (SqlParameter statusCode, SqlParameter statusMessage) CreateOutputParameters()
        {
            return (
                new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("@StatusMessage", SqlDbType.NVarChar, 250) { Direction = ParameterDirection.Output }
            );
        }
        private MenuActionResult ParseActionResult(SqlParameter statusCodeParam, SqlParameter statusMessageParam)
        {
            int statusCode = (statusCodeParam.Value != DBNull.Value) ? Convert.ToInt32(statusCodeParam.Value) : -1;
            string statusMessage = (statusMessageParam.Value != DBNull.Value) ? statusMessageParam.Value.ToString()! : "Unknown Status";
            return new MenuActionResult { StatusCode = statusCode, StatusMessage = statusMessage };
        }
        public class MenuActionResult
        {
            public int StatusCode { get; set; }
            public string StatusMessage { get; set; } = string.Empty;

        }
        
    
    }
}

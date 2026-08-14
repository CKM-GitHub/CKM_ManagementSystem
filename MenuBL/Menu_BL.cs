using CKM_ManagementSystem.DL;
using System.Data;
using Microsoft.Data.SqlClient;

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
            bool status)
        {
            if (string.IsNullOrWhiteSpace(menuName))
            {
                return new MenuActionResult
                {
                    StatusCode = 0,
                    StatusMessage = "Menu Name is required."
                };
            }
            if (!displayOrder.HasValue || displayOrder.Value <0 || displayOrder.Value > 999)
            {
                return new MenuActionResult
                {
                    StatusCode = 0,
                    StatusMessage = "Display Order must be a 3-digit number (eg. 000 to 999) ."
                };
            }
            
            var statusCodeParam = new SqlParameter
            {
                ParameterName = "@StatusCode",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };
            var statusMessageParam = new SqlParameter
            {
                ParameterName = "@StatusMessage",
                SqlDbType = SqlDbType.NVarChar,
                Size = 250,
                Direction = ParameterDirection.Output
            };
            var parameters = new[]
            {
                new SqlParameter ("@MenuName", (object?)menuName ?? DBNull.Value),
                new SqlParameter ("@ActionName", (object?)actionName ?? DBNull.Value),
                new SqlParameter ("@ControllerName",(object?)controllerName ?? DBNull.Value),
                new SqlParameter ("@MenuIcon", string.IsNullOrWhiteSpace(menuIcon) ? DBNull.Value : menuIcon),
                new SqlParameter ("@DisplayOrder", displayOrder.Value),
                new SqlParameter ("@ParentMenuId", (object?)parentMenuId ?? DBNull.Value),
                new SqlParameter("@Status", status),
                statusCodeParam,
                statusMessageParam
            };
            await ExecuteAsync("sp_CreateMenu", parameters);

            return new MenuActionResult
            {
                StatusCode = statusCodeParam.Value != DBNull.Value
                    ? Convert.ToInt32(statusCodeParam.Value) : 0,
                StatusMessage = statusMessageParam.Value?.ToString() ?? string.Empty
            };
        }

        public class MenuActionResult
        {
            public int StatusCode { get; set; }
            public string StatusMessage { get; set; } = string.Empty;

        }
        public class MenuListItem
        {
            public int MenuID { get; set; }
            public string MenuName { get; set; } = string.Empty;
            public string? IconClass { get; set; }
            public string ControllerName { get; set; } = string.Empty;
            public string ActionName { get; set; } = string.Empty;
            public string Route { get; set; } = string.Empty;

            public int? ParentMenuId { get; set; }
            public string ParentMenuName { get; set; } = "Main Menu";
            public int DisplayOrder { get; set; }
            public bool Status { get; set; }
        }
    
    }
}

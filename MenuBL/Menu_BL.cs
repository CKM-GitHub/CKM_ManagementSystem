using DL;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MenuBL
{
    public class Menu_BL : BaseDL
    {
        public Menu_BL(string connectionString, int commandTimeout= 30)
            :base(connectionString, commandTimeout){ }

        public async Task<MenuActionResult> CreateMenuAsync(
            string? menuName,
            string? actionName,
            string? controllerName,
            string? menuIcon,
            string? displayOrder,
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
            int parsedDisplayOrder;
            if(string.IsNullOrWhiteSpace(displayOrder) ||
                displayOrder.Trim().Length != 3 ||
                !int.TryParse(displayOrder, out parsedDisplayOrder))
            {
                return new MenuActionResult
                {
                    StatusCode = 0,
                    StatusMessage = "Display Order must be  eg. 000, 100."
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
                new SqlParameter ("@DisplayOrder", displayOrder),
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

        public async Task<List<MenuListItem>> GetMenuListAsync(string? searchTerm, int? parentMenuId)
        {
            var parameters = new[]
            {
                new SqlParameter("@SearchTerm", string.IsNullOrWhiteSpace(searchTerm) ? DBNull.Value : searchTerm),
                new SqlParameter("@ParentMenuId", (object?)parentMenuId ?? DBNull.Value)
            };

            DataTable dt = await SelectDataTableAsync("sp_GetMenuList", parameters);
            var menuList = new List<MenuListItem>();

            foreach (DataRow row in dt.Rows)
            {
                string controllerName = row["ControllerName"] != DBNull.Value ? row["ControllerName"].ToString()! : string.Empty;
                string actionName = row["ActionName"] != DBNull.Value ? row["ActionName"].ToString()! : string.Empty;

                menuList.Add(new MenuListItem
                {
                    MenuID = Convert.ToInt32(row["MenuID"]),
                    MenuName = row["MenuName"] != DBNull.Value ? row["MenuName"].ToString()! : string.Empty,
                    IconClass = row["MenuIcon"] != DBNull.Value ? row["MenuIcon"].ToString() : null,

                    ControllerName = controllerName,
                    ActionName = actionName,
                    Route = !string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName)
                             ? $"/{controllerName}/{actionName}".ToLower()
                             : "#",
                    ParentMenuName = row["ParentMenuName"] != DBNull.Value ? row["ParentMenuName"].ToString()! : "Main Menu",
                    DisplayOrder = row["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(row["DisplayOrder"]) : 0,
                    Status = row["Status"] != DBNull.Value && Convert.ToBoolean(row["Status"])
                });

            }
            return menuList;
        }
        public async Task<List<MenuListItem>> GetParentMenusForDropdownAsync()
        {
            var allMenus = await GetMenuListAsync(searchTerm: null, parentMenuId: null);
            var parentMenus = allMenus
                .Where(m => m.ParentMenuName == "Main Menu")
                .ToList();

            return parentMenus;
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
            public string ParentMenuName { get; set; } = "Main Menu";
            public int DisplayOrder { get; set; }
            public bool Status { get; set; }
        }
    
    }
}

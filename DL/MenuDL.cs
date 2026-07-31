using System.Data;
using Microsoft.Data.SqlClient;
namespace DL
{
    public class MenuDL : BaseDL
    {
        public MenuDL(string connectionString, int commandTimeout = 30)
            : base(connectionString, commandTimeout) { }

        public async Task<MenuActionResult> CreateMenuAsync(
            string? menuName,
            string? actionName,
            string? controllerName,
            string? menuIcon,
            int displayOrder,
            int? parentMenuId,
            bool status)
        {
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
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CreateMenu", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = _commandTimeout;
            command.Parameters.AddRange(parameters);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

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
        public async Task<List<MenuListItem>> GetMenuListAsync(string? serachTerm, int? parentMenuId)
        {
            var parameters = new[]
            {
                new SqlParameter("@SearchTerm", string.IsNullOrWhiteSpace(serachTerm) ? DBNull.Value : serachTerm),
                new SqlParameter("@ParentMenuId", (object?)parentMenuId ?? DBNull.Value)
            };
            foreach(DataRow row in dt.Rows)
            {
                string controllerName = row["ControllerName"] != DBNull.Value ? row["ControllerName"].ToString()! : string.Empty;
                string actionName = row["ActionName"] != DBNull.Value ? row["ActionName"].ToString()! : string.Empty;

                GetMenuListAsync().Add(new MenuListItem
                {
                    parentMenuId = Convert.ToInt32(row["MenuID"]),
                    MenuName = row["MenuName"] != DBNull.Value ? row["MenuName"].ToString()! : string.Empty,
                    MenuIcon = row["MenuIcon"] != DBNull.Value ? row["MenuIcon"].ToString() : null,

                    Route = !string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName)
                            ? $"/{controllerName}/{actionName}".ToLower()
                            : "#",
                    parentMenuName = row["ParentMenuName"] != DBNull.Value ? row["ParentMenuName"].ToString()! : "Main Menu",
                    DisplayOrder = row["DisplayOrder"] != DBNull ? Convert.ToInt32(row["DisplayOrder"]) : 0,
                    Status = row["Status"] != DBNull.Value && Convert.ToBoolean(row["Status"])
                });

            }
            return GetMenuListAsync();
        }
    }
}

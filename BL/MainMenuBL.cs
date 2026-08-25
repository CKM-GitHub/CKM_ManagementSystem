using System.Data;
using Microsoft.Data.SqlClient;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.ViewModels.MainMenu;

namespace CKM_ManagementSystem.BL
{
    public class MainMenuBL
    {
        private readonly BaseDL _baseDL;

        public MainMenuBL(BaseDL baseDL)
        {
            _baseDL = baseDL;
        }

        public List<MainMenuViewModel> GetMainMenus(string staffCode)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@StaffCode",staffCode)
            };

            DataTable dataTable = _baseDL.GetData("Menu_GetMainMenu", parameters);

            List<MainMenuViewModel> menuList = new List<MainMenuViewModel>();

            foreach (DataRow row in dataTable.Rows)
            {
                MainMenuViewModel menu = new MainMenuViewModel
                {
                    MenuID = Convert.ToInt32(row["MenuID"]),
                    MenuName = row["MenuName"].ToString() ?? string.Empty,
                    ActionName = row["ActionName"].ToString() ?? string.Empty,
                    ControllerName = row["ControllerName"].ToString() ?? string.Empty,
                    MenuIcon = row["MenuIcon"] == DBNull.Value
                    ? null : row["MenuIcon"].ToString(),
                    DisplayOrder = Convert.ToInt32(row["DisplayOrder"]),
                    ParentMenuId = row["ParentMenuId"] == DBNull.Value
                    ? null : Convert.ToInt32(row["ParentMenuId"]),

                    UserName = row["UserName"].ToString()??string.Empty,
                    ImageURL= row["ImageURL"]==DBNull.Value
                    ? null : row["ImageURL"].ToString(),
                    RoleName = row["RoleName"].ToString()??string.Empty
                };
                menuList.Add(menu);
            }

            List<MainMenuViewModel>parentMenus = menuList
                .Where(menu=>menu.ParentMenuId == null)
                .OrderBy(menu=>menu.DisplayOrder)
                .ToList();

            foreach(MainMenuViewModel parentMenu in parentMenus)
            {
                parentMenu.SubMenus=menuList
               .Where(menu=> menu.ParentMenuId == parentMenu.MenuID)
               .OrderBy(menu=>menu.DisplayOrder) 
               .ToList();

            }

            return parentMenus;
        }
    }
}

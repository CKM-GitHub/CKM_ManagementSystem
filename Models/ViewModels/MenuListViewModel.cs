using Microsoft.AspNetCore.Mvc.Rendering;
namespace CKM_ManagementSystem.Models.ViewModels
{
        public class MenuListItem
        {
            public int MenuID { get; set; }
            public string DisplayText { get; set; } = string.Empty;
            public string? IconClass { get; set; }
            public string ControllerName { get; set; } = string.Empty;
            public string ActionName { get; set; } = string.Empty;
            public string Route { get; set; } = string.Empty;
            public string MenuType { get; set; } = "Main Menu";
            public int DisplayOrder { get; set; } 
            public bool Status { get; set; }
        }
        public class MenuListViewModel
        {
            public string? SearchTerm { get; set; }
            public int? SelectedParentId { get; set; }
            public List<SelectListItem> ParentMenuList { get; set; } = new();
            public List<MenuListItem> Menus { get; set; } = new();

            public int CurrentPage { get; set; } = 1;
            
            public int TotalPages { get; set; }
            public int TotalItems { get; set; }
            public int PageSize { get; set; } 

        }
    
}

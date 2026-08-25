namespace CKM_ManagementSystem.Models.ViewModels.MainMenu
{
    public class MainMenuViewModel
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; }= string.Empty;

        public string ActionName { get; set; } = string.Empty;

        public string ControllerName { get; set; } = string.Empty;

        public string MenuIcon {  get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public int? ParentMenuId { get; set; }

        public string UserName { get; set; } =string.Empty;

        public string? ImageURL { get; set; }

        public string RoleName { get; set; }= string.Empty;

        public List<MainMenuViewModel> SubMenus { get; set; } = new();
    }
}

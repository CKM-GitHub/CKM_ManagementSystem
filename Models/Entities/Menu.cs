namespace CKM_ManagementSystem.Models.Entities
{
    public class Menu
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string ControllerName { get; set; } = string.Empty;
        public string? MenuIcon {  get; set; }
        public int DisplayOrder {  get; set; }
        public int? ParentMenuId { get; set; }
        public DateTime? Created_Date { get; set; }
        public DateTime? Updated_Date { get; set; }
        public DateTime? Deleted_Date { get; set; }
        public bool Status { get; set; } = true;
    }
}

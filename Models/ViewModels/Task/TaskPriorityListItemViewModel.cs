namespace CKM_ManagementSystem.Models.ViewModels.Task
{
    public class TaskPriorityListItemViewModel
    {
        public string Priority_Code { get; set; } = string.Empty;
        public string Priority_Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
    }
}

namespace CKM_ManagementSystem.Models.ViewModels.Announcements
{
    public class AnnouncementListFilterViewModel
    {
        public string? SearchText { get; set; }

        public int? Priority { get; set; }

        public DateTime? AnnouncedDate { get; set; } 
    }
}

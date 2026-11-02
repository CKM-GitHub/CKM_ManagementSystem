namespace CKM_ManagementSystem.Models.ViewModels.Announcements
{
    public class AnnouncementDetailViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string AnnouncedBy { get; set; } = string.Empty;

        public DateTime AnnouncedDate { get; set; }

        public int Priority { get; set; }

        public string Target { get; set; } = string.Empty;

        public string AnnouncerDepartment { get; set; } = string.Empty;
    }
}
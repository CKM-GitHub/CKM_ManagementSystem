namespace CKM_ManagementSystem.Models.ViewModels.Announcements
{
    public class AnnouncementListItemViewModel
    {
        public int ID { get;set; }
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string AnnouncedBy { get;set; } = string.Empty;

        public DateTime AnnouncedDate { get; set; }

        public string Target { get; set; } = string.Empty;

        public int Priority { get; set; }
    }
}

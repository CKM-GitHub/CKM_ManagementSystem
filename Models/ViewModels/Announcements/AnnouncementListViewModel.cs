namespace CKM_ManagementSystem.Models.ViewModels.Announcements
{
    public class AnnouncementListViewModel
    {
        public AnnouncementListFilterViewModel Filter { get; set; }
            = new();

        public List<AnnouncementListItemViewModel> Announcements { get; set; }
            = new();

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 6;

        public int TotalRecords { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling(
                (double)TotalRecords / PageSize
            );
    }
}

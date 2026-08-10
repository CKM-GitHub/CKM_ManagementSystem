namespace CKM_ManagementSystem.Models.ViewModels
{
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; } = new();
        public int OverallTotalCount { get; set; }
        public int OverallActiveCount { get; set; }
        public int OverallInactiveCount { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages =>
            PageSize > 0
                ? (int)Math.Ceiling((double)TotalCount / PageSize)
                : 0;
        public int ErrorCode { get; set; }
        public bool HasError => ErrorCode != 0;
    }
}
namespace CKM_ManagementSystem.Models.ViewModels.Common
{
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; } = new();

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages =>
            PageSize > 0
                ? (int)Math.Ceiling((double)TotalCount / PageSize)
                : 0;
    }
}
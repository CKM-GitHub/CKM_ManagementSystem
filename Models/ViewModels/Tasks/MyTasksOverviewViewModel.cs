namespace CKM_ManagementSystem.Models.ViewModels.Tasks
{
    public class MyTasksOverviewViewModel
    {
        public MyTaskFilterViewModel Filter { get; set; }
            = new MyTaskFilterViewModel();

        public List<TaskLookupItemViewModel> ProjectList { get; set; }
            = new List<TaskLookupItemViewModel>();

        public List<TaskLookupItemViewModel> PriorityList { get; set; }
            = new List<TaskLookupItemViewModel>();

        public List<TaskLookupItemViewModel> StatusList { get; set; }
            = new List<TaskLookupItemViewModel>();

        public List<MyTaskListItemViewModel> TaskList { get; set; }
            = new List<MyTaskListItemViewModel>();

        public List<MyTaskStatusSummaryViewModel> StatusSummary { get; set; }
            = new List<MyTaskStatusSummaryViewModel>();

        public MyTaskUpdateViewModel UpdateTask { get; set; }
            = new MyTaskUpdateViewModel();


        // Pagination
        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public int TotalItems { get; set; }

        public int TotalPages =>
            PageSize <= 0
                ? 0
                : (int)Math.Ceiling(
                    (double)TotalItems / PageSize
                );
    }
}
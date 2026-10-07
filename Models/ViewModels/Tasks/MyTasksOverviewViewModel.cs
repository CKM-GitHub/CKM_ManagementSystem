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
    }
}
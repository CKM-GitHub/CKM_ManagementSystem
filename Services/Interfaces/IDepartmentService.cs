using CKM_ManagementSystem.Models.ViewModels;

namespace CKM_ManagementSystem.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<bool> IsDepartmentCodeDuplicateAsync(string departmentCode);

        Task<bool> IsDepartmentNameDuplicateAsync(string departmentName);
        Task CreateAsync(DepartmentEntryViewModel model);
        }
}
    
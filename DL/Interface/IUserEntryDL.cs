using CKM_ManagementSystem.Models;
using CKM_ManagementSystem.Models.ViewModels;

namespace CKM_ManagementSystem.DL.Interface
{
    public interface IUserEntryDL
    {
        Task<int> CreateUserAsync(UserCreateViewModel model);
        Task<IEnumerable<Department>> GetDepartmentsAsync();
        Task<IEnumerable<UserRole>> GetUserRolesAsync();
    }
}

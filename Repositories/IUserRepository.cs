using CKM_ManagementSystem.Models;

namespace CKM_ManagementSystem.Repositories
{
    public interface IUserRepository
    {
        Task<int> CreateUserAsync(User user);

        Task<IEnumerable<Department>> GetActiveDepartmentsAsync();

        Task<IEnumerable<UserRole>> GetUserRolesAsync();
    }
}
using CKM_ManagementSystem.Models.ViewModels.Password;

namespace CKM_ManagementSystem.DL.Interface
{
    public interface IChangePasswordDL
    {
        Task<string?> GetCurrentPasswordAsync(string StaffCode);
        Task<int> ChangePasswordAsync(ChangePasswordViewModel model);   
    }
}

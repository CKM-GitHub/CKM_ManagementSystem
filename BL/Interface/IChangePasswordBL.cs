using CKM_ManagementSystem.Models.ViewModels.Password;

namespace CKM_ManagementSystem.BL.Interface
{
    public interface IChangePasswordBL
    {
        Task<int> ChangePasswordAsync(ChangePasswordViewModel model);
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;

namespace CKM_ManagementSystem.Models.ViewModels
{
    public class EditMenuViewModel : CreateMenuViewModel
    {
        public int MenuID {  get; set; }
    }
}

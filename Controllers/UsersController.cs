using Microsoft.AspNetCore.Mvc;

namespace CKM_ManagementSystem.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult UserList()
        {
            return View();
        }
    }
}

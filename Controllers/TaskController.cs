using Microsoft.AspNetCore.Mvc;

namespace CKM_ManagementSystem.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace CKM_ManagementSystem.Controllers
{
    public class ProjectController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace ElNotebook.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

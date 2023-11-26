using ElNotebook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElNotebook.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private ApplicationContext db;
        public StudentController(ApplicationContext context)
        {
            db = context;

        }
        public IActionResult Index()
        {
            var id = Convert.ToInt32(User.FindFirst(ClaimTypes.Surname)?.Value);
            var cources = db.Courses.ToList();
            var student = db.Students.ToList().Find(s => s.UserId == id);
            return View((cources, student));
        }
    }
}

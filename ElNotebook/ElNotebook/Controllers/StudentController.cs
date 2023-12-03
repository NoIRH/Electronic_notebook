using ElNotebook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
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
        [HttpPost]
        public IActionResult ShowActiveCourse()
        {
            var id = Convert.ToInt32(User.FindFirst(ClaimTypes.Surname)?.Value);
            db.Students.Include(c => c.Activities);
            var student = db.Students.ToList().Find(s => s.UserId == id);
            var cources = student?.Activities.Where(s => s.Activity == ActivityType.Closed);

            return View((cources, student));
        }
        [HttpPost]
        public IActionResult ShowClosedCourse()
        {
            var id = Convert.ToInt32(User.FindFirst(ClaimTypes.Surname)?.Value);
            db.Students.Include(c => c.Activities);
            var student = db.Students.ToList().Find(s => s.UserId == id);
            var cources = student?.Activities.Where(s => s.Activity == ActivityType.Closed);

            return View((cources, student));
        }
        public IActionResult EditProfile(int? id)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(User user)
        {
            db.Users.Update(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult SubscribeOnCourse() 
        {
            return View();
        }
        public IActionResult UnSubscribeOnCourse()
        {
            return View();
        }
        public async Task<IActionResult> CourseDetails(int? id)
        {
            if (id != null)
            {
                Course? course = await db.Courses.FirstOrDefaultAsync(p => p.Id == id);
                if (course != null) return View(course);
            }
            return NotFound();
        }
    }
}

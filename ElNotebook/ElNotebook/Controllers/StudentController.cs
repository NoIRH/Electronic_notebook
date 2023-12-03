using ElNotebook.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
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
        public IActionResult ShowActiveCourse()
        {
            var id = Convert.ToInt32(User.FindFirst(ClaimTypes.Surname)?.Value);
            var student = db.Students.Include(c => c.Courses).
                ToList().Find(s => s.UserId == id);
            var cources = student?.Activities.
                Where(s => s.Activity == ActivityType.Active).
                Select(s => s.Course)
                .ToList();
            return View((cources, student));
        }
        public IActionResult ShowClosedCourse()
        {
            var id = Convert.ToInt32(User.FindFirst(ClaimTypes.Surname)?.Value);
            db.Students.Include(c => c.Courses);
            var student = db.Students.ToList().Find(s => s.UserId == id);
            var cources = student?.Activities.
                Where(s => s.Activity == ActivityType.Closed).
                Select(s => s.Course)
                .ToList();
            return View((cources, student));
        }
        public IActionResult EditStudentProfile(int? id)
        {
            var st = db.Students.FirstOrDefault(p => p.UserId == id);
            if (st != null)
                return View(st);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ActionResult> EditStudentProfile(Student student)
        {
            db.Students.Update(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult EditUserProfile(int? id)
        {
            if (id != null)
            {
                var user = db.Users.FirstOrDefault(p => p.Id == id);
                if (user != null) return View(user);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditUserProfile(User user)
        {
            db.Users.Update(user);
            db.SaveChanges();
            await Request.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = new List<Claim>
            {
                 new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString()),
                new Claim(ClaimTypes.Surname, user.Id.ToString()),
             };
            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await Request.HttpContext.SignInAsync(claimsPrincipal);
            return RedirectToAction("Index");
        }
        public IActionResult SubscribeOnCourse(int? id) 
        {
            if (id != null)
            {
                var c = db.Courses.FirstOrDefault(p => p.Id == id);
                if (c != null)
                {
                    if (User.FindFirst(ClaimTypes.Surname)?.Value != null)
                    {
                        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.Surname)?.Value);
                        var student = db.Students.Include(s => s.Activities).
                            FirstOrDefault(s => s.UserId == userId);
                        if (student is not null)
                        {
                            if(student?.Activities.Where(a => a.Course?.Id == c.Id ).Count() == 0)
                                student?.Activities.
                                    Add(
                                    new StudentCoursesActivity 
                                    {   Activity = ActivityType.Active,
                                        Course = c,
                                        CourseId = c.Id,
                                        Student = student,
                                        StudentId = student.Id
                                    });
                                db.SaveChanges();
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult UnSubscribeOnCourse(int? id)
        {
            if (id != null)
            {
                var c = db.Courses.FirstOrDefault(p => p.Id == id);
                if (c != null)
                {
                    if (User.FindFirst(ClaimTypes.Surname)?.Value != null)
                    {
                        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.Surname)?.Value);
                        var student = db.Students.Include(s => s.Courses).
                            FirstOrDefault(s => s.UserId == userId);
                        if (student is not null)
                        {
                            if (student?.Activities.
                                Where(a => a.Course?.Id == c.Id && a.Activity != ActivityType.Closed).
                                Count() != 0)
                            {
                                student?.Courses.Remove(c);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index");
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

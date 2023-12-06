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
    public class StudentController : Controller
    {
        private ApplicationContext db;
        public StudentController(ApplicationContext context)
        {
            db = context;
        }
        [Authorize(Roles = "Student")]
        public IActionResult Index()
        {
            var id = Convert.ToInt32(User.FindFirst(ClaimTypes.Surname)?.Value);
            var cources = db.Courses.ToList();
            var student = db.Students.ToList().Find(s => s.UserId == id);
            return View((cources, student));
        }
        [Authorize(Roles = "Student")]
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
        [Authorize(Roles = "Student")]
        public IActionResult ShowClosedCourse()
        {
            var id = Convert.ToInt32(User.FindFirst(ClaimTypes.Surname)?.Value);
            var student = db.Students.Include(c => c.Courses).ToList().Find(s => s.UserId == id);
            var cources = student?.Activities.
                Where(s => s.Activity == ActivityType.Closed).
                Select(s => s.Course)
                .ToList();
            return View((cources, student));
        }
        [Authorize(Roles = "Student")]
        public IActionResult EditStudentProfile(int? id)
        {
            var st = db.Students.FirstOrDefault(p => p.Id == id);
            if (st != null)
                return View(st);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> EditStudentProfile(Student student)
        {
            db.Students.Update(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Student")]
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
        [Authorize(Roles = "Student")]
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
        [Authorize(Roles = "Student")]
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
        [Authorize(Roles = "Student")]
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
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CourseDetails(int? id)
        {
            if (id != null)
            {
                Course? course = await db.Courses.FirstOrDefaultAsync(p => p.Id == id);
                if (course != null) return View(course);
            }
            return NotFound();
        }
        [Authorize(Roles ="Student, Admin, Manager")]
        public async Task<IActionResult> StudentDetails(int? id)
        {
            if (id != null)
            {
                db.Activities.Load();
                db.Students.Load();
                db.Courses.Load();
                var st = await db.Students.Include(s => s.Activities).FirstOrDefaultAsync(p => p.Id == id);
                if (st!= null) return View(st);
            }
            return NotFound();
        }
    }
}

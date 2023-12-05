using ElNotebook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElNotebook.Controllers
{
    [Authorize(Roles ="Manager,Admin")]
    public class ManagerController : Controller
    {
        private ApplicationContext db;
        public ManagerController(ApplicationContext db)
        {
            this.db = db;
        }
        public  IActionResult Index()
        {
            var cources = db.Courses.ToList();
            var students = db.Students.Include(s => s.Activities).ToList();
            return View((cources,students));
        }
        public IActionResult ShowAll()
        {
            return Redirect("Index");
        }
        public IActionResult CloseCourse(int? courseId, int? studentId)
        {
            if (courseId != null)
            {
                Course? course =  db.Courses.
                    Include(c => c.Students).
                    FirstOrDefault(p => p.Id == courseId);
                if (course != null && studentId !=  null)
                {
                    var a = course.Activities.FirstOrDefault(a => a.StudentId == studentId);
                    if (a is not null)
                    {
                        a.Activity = ActivityType.Closed;
                        db.Update(a);
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("ShowCourseStudent","Manager",new {id = courseId });
        }
        public IActionResult ActiveCourse(int? courseId, int? studentId)
        {
            if (courseId != null)
            {
                Course? course = db.Courses.
                    Include(c => c.Students).
                    FirstOrDefault(p => p.Id == courseId);
                if (course != null && studentId != null)
                {
                    var a = course.Activities.FirstOrDefault(a => a.StudentId == studentId);
                    if (a is not null)
                    {
                        a.Activity = ActivityType.Active;
                        db.Update(a);
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("ShowCourseStudent", "Manager", new { id = courseId });
        }
        public IActionResult ShowCourseStudent(int? id)
        {
            db.Courses.Load();
            var cource = db.Courses.Include(c =>c.Students).
                ThenInclude(s => s.Activities).
                FirstOrDefault(c => c.Id == id);
            var students = cource?.Students;
            return View((cource, students));
        }
        public IActionResult CreateCourse()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            db.Courses.Add(course); 
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int? id)
        {
            if (id != null)
            {
                Course? course = await db.Courses.FirstOrDefaultAsync(p => p.Id == id);
                if (course != null)
                {
                    db.Courses.Remove(course);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }
        public async Task<IActionResult> EditCourse(int? id)
        {
            if (id != null)
            {
                Course? course = await db.Courses.FirstOrDefaultAsync(p => p.Id == id);
                if (course != null)  return  View(course); 
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditCourse(Course course) 
        {
            db.Courses.Update(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

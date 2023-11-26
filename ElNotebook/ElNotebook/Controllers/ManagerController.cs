using ElNotebook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElNotebook.Controllers
{
   
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
            var students = db.Students.ToList();
            return View((cources,students));
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

using ElNotebook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElNotebook.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private ApplicationContext db;
        public AdminController(ApplicationContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            var users = db.Users.ToList();  
            return View(users);
        }
        public IActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddUser(User user)
        {
            var u = db.Users.Find(user.Id);
            
            if (u == null && user != null && user.Name != null && user.Password != null)
            {
                db.Users.Add(user);
                db.SaveChanges();
                if (user.Role == Roletype.Student)
                {
                    Student st = null;
                    u = db.Users.Find(user.Id);
                    st = new Student { Name = user.Name, UserId = u.Id };
                    db.Students.Add(st);
                }
                if (user.Role == Roletype.Manager)
                {
                    Manager m = null;
                    u = db.Users.Find(user.Id);
                    m = new Manager { Name = user.Name, UserId = u.Id };
                    db.Managers.Add(m);
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int? id)
        {
            if (id != null)
            {
                var user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null)
                {
                    if(user.Role == Roletype.Manager)
                    {
                        var m = await db.Managers.
                            FirstOrDefaultAsync(m => m.UserId == id);
                        if (m != null)
                        {
                            db.Managers.Remove(m);
                        }
                    }
                    if (user.Role == Roletype.Student)
                    {
                        var s = await db.Students.
                            FirstOrDefaultAsync(m => m.UserId == id);
                        if (s != null)
                        {
                            db.Students.Remove(s);
                        }
                    }
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }
        public async Task<IActionResult> EditUser(int? id)
        {
            if (id != null)
            {
                var user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null) return View(user);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(User user)
        {
            db.Users.Update(user);
            db.SaveChanges();
            if (user.Role == Roletype.Student)
            {
                Student st = null;
                var s = db.Students.FirstOrDefault(s => s.UserId == user.Id);
                if (s is null)
                {
                    st = new Student { Name = user.Name, UserId = user.Id };
                    db.Students.Add(st);
                }
            }
            if (user.Role == Roletype.Manager)
            {
                Manager m = null;
                var mch = db.Managers.FirstOrDefault(s => s.UserId == user.Id);
                if (mch is null)
                {
                    m = new Manager { Name = user.Name, UserId = user.Id };
                    db.Managers.Add(m);
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

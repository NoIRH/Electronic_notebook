using ElNotebook.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Security.Claims;

namespace ElNotebook.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;
        public HomeController(ApplicationContext context)
        {
            db = context;
            if (db.Users.Where(u => u.Role == Roletype.Admin).Count() == 0)
            {
                db.Users.Add(
                    new User
                    {
                        Name = "admin",
                        Password = "admin",
                        Email = "admin",
                        Role = Roletype.Admin,
                    });
                db.SaveChanges();
            }
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Reg()
        {
            return View();
        }
        public IActionResult EditStudentProfile()
        {
            var id = (int?)TempData["user_id"];
            var st = db.Students.FirstOrDefault(p => p.UserId == id);
            if (st != null) 
                return View(st);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ActionResult> Reg(User user)
        {
            var u = db.Users.Find(user.Id);
            Student st  = null;
            var hasNameDuplicate = db.Users.Where(u => u.Name == user.Name).Count() != 0;
            if (u == null && 
                user != null && 
                user.Name != null && 
                user.Password !=  null &&
                !hasNameDuplicate)
            {
                user.Role = Roletype.Student;
                db.Users.Add(user);
                db.SaveChanges();
                u = db.Users.Find(user.Id);
                st = new Student { Name = user.Name, UserId = u.Id };
                db.Students.Add(st);
                db.SaveChanges();
                TempData["user_id"] = u.Id;
            }
            if(st is not null)
                 return RedirectToAction("EditStudentProfile", "Home");
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ActionResult> EditStudentProfile(Student student)
        {
            db.Students.Update(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ActionResult> Login(User user)
        {
            if (user == null) return RedirectToAction("Index");
            var u = db.Users.FirstOrDefaultAsync(u => u.Name == user.Name && u.Password == user.Password).Result;
            if (u == null ) return RedirectToAction("Index");
            var claims = new List<Claim>
            {
                 new Claim(ClaimsIdentity.DefaultNameClaimType, u.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, u.Role.ToString()),
                new Claim(ClaimTypes.Surname, u.Id.ToString()),
             };
            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await Request.HttpContext.SignInAsync(claimsPrincipal);
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<ActionResult> Logout()
        {
            await Request.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
    }
}
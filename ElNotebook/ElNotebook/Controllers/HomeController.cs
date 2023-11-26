using ElNotebook.Models;
using Microsoft.AspNetCore.Authentication;
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

        }

        public IActionResult Index()
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
        [HttpPost]
        public async Task<ActionResult> Reg(User user)
        {
            var u = db.Users.Find(user.Id); 
            if (u == null && user != null && user.Name != null && user.Password !=  null)
            {
                user.Role = Roletype.Student;
                db.Users.Add(user);
                db.SaveChanges();
                u = db.Users.Find(user.Id);
                var st = new Student { Name = user.Name, UserId = u.Id };
                db.Students.Add(st);
                db.SaveChanges();
            }
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
    }
}
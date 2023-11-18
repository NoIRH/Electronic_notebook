using ElNotebook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ElNotebook.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;
        public HomeController(ApplicationContext context)
        {
            db = context;

            
            //s1.CourcesActive.Add(c1);
            //s1.CourcesClosed.Add(c2);
            //s2.CourcesActive.Add(c2);
            //s2.CourcesClosed.Add(c1);
           // db.Cources.AddRange(c1, c2);
          //  db.Students.AddRange(s1, s2);
            

           
            
        }

        public IActionResult Index()
        {
            return View();
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
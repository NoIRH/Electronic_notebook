using ElNotebook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ElNotebook.Controllers
{
    public class AuthRegController : Controller
    {
        public static User Authorize(string login, string password)
        {
            //return ManagerOperation.Manager.GetUsers().Where(u => u.Name == login && u.Password == password).FirstOrDefault();
            throw new NotImplementedException(); 
        }

        public static User Register(string login, string password)
        {
            //if (ManagerOperation.Manager.GetUsers().Where(u => u.Name == login && u.Password == password).Count() != 0)
            //    new DuplicateNameException();
            //User user = new User() { Name = login, Password = password };
            //ManagerOperation.Manager.AddUser(user);
            //return user;
            throw new NotImplementedException();
        }
    }
}

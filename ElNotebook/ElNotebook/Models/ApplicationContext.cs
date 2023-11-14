using Microsoft.EntityFrameworkCore;

namespace ElNotebook.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Cource> Cources { get; set; } 
        public DbSet<User> Users { get; set; } 
        public DbSet<Student> Students { get; set; } 
        public DbSet<Manager> Managers { get; set; } 
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}

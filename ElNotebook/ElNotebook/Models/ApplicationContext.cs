using Microsoft.EntityFrameworkCore;

namespace ElNotebook.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Course> Courses { get; set; } 
        public DbSet<User> Users { get; set; } 
        public DbSet<Student> Students { get; set; } 
        public DbSet<Manager> Managers { get; set; } 
        public DbSet<StudentCoursesActivity> Activities { get; set; }   
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Course>()
                .HasMany(c => c.Students)
                .WithMany(s => s.Courses)
                .UsingEntity<StudentCoursesActivity>(
                   a => a
                    .HasOne(pt => pt.Student)
                    .WithMany(t => t.Activities)
                    .HasForeignKey(pt => pt.StudentId),
                   a => a
                    .HasOne(pt => pt.Course)
                    .WithMany(p => p.Activities)
                    .HasForeignKey(pt => pt.CourseId)
               );
        }
    }
   
}

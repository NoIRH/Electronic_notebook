using System.ComponentModel.DataAnnotations.Schema;

namespace ElNotebook.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public int? TotalCountOFhours {
            get
            {
                return Activities?.
                    Where(a => a.Activity == ActivityType.Closed).
                    Select(a => a?.Course?.CountOFhours).Sum();
            }
        }
        public int? NumberGroup { get; set; }
        public List<Course> Courses { get; set; } = new();
        public List<StudentCoursesActivity> Activities { get; set; } = new();
        public User? User { get; set; }   
        public int? UserId { get; set; }
    }
}

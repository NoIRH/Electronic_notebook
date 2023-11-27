namespace ElNotebook.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? TotalCountOFhours { get; set; }
        public int? NumberGroup { get; set; }
        public List<Course> Courses { get; set; } = new();
        public List<StudentCoursesActivity> Activities { get; set; } = new();
        public int? UserId { get; set; }   
    }
}

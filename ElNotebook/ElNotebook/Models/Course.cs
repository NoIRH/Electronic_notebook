namespace ElNotebook.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public int? CountOFhours { get; set; }
        public List<Student> Students { get; set; } = new();
        public List<StudentCoursesActivity> Activities { get; set; } = new();   
    }
}

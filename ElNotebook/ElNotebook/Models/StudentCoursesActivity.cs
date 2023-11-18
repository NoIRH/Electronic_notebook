namespace ElNotebook.Models
{
    public enum ActivityType{
        None=0,
        Active =1,
        Closed =2
    }
    public class StudentCoursesActivity
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public int CourseId { get; set; }
        public Course? Course { get; set; }
        public ActivityType Activity { get; set; } = ActivityType.None;  
    }
}

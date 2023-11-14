namespace ElNotebook.Models
{
    public class Cource
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public int? CountOFhours { get; set; }
        public List<User?>? ActiveStudents { get; set; }
    }
}

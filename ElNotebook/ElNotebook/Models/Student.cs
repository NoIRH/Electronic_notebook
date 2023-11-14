namespace ElNotebook.Models
{
    public class Student: User
    {
        public int? TotalCountOFhours { get; set; }
        public int? NumberGroup { get; set; }
        public List<Cource>? CourcesActive { get; set; } = new();
        public List<Cource>? CourcesClosed { get; set; } = new();
    }
}

namespace ElNotebook.Models
{
    public enum Roletype
    {
        None = 0,       
        Admin = 1,
        Manager = 2,
        Student = 4
    }
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Roletype Role { get; set; } = Roletype.None;

    }
}

namespace MuranGameProject.Models
{
    public class UserGroup
    {
        public string Username { get; set; } = "unknown";
        public string Group { get; set; }
        public string ConnId { get; set; }
        public string NextPlayerConnId { get; set; }
        public string[] Hand { get; set; }
        public int Place { get; set; }
    }
}
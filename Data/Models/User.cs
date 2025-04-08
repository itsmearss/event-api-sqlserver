using System.ComponentModel.DataAnnotations;

namespace TestProjectAnnur.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Password { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<RegisterEvent> RegisterEvents { get; } = new List<RegisterEvent>();
    }
}

namespace TestProjectAnnur.Data.Models
{
    public class RegisterEvent
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public bool IsAttend { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Certificate? Certificate { get; set; }
    }
}

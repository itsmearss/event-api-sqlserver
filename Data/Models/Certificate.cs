namespace TestProjectAnnur.Data.Models
{
    public class Certificate
    {
        public int Id { get; set; }
        public string File { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int? RegisterEventId { get; set; }
        public RegisterEvent? RegisterEvent { get; set; }
    }
}

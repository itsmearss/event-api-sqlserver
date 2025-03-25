using TestProjectAnnur.Data.Models;

namespace TestProjectAnnur.Data.DTOs
{
    public class EventDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public int MaxAttendees { get; set; }
        public Status Status { get; set; }
        public string Flyer { get; set; }
        public string Cover { get; set; }
    }

    public class EventResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public int MaxAttendees { get; set; }
        public Status Status { get; set; }
        public string Flyer { get; set; }
        public string Cover { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime?UpdatedAt { get; set; }
    }
}

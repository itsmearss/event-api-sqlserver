using TestProjectAnnur.Data.Models;

namespace TestProjectAnnur.Data.DTOs
{
    public class RegisterEventDTO
    {
        public int? EventId { get; set; }
        public int? UserId { get; set; }
        public AttendStatus? IsAttend { get; set; }
        public IFormFile? Certificate { get; set; }
        public string? CertificateName { get; set; }
    }

    public class RegisterEventResponseDTO
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? EventId { get; set; }
        public AttendStatus? IsAttend { get; set; }
        public string? Certificate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

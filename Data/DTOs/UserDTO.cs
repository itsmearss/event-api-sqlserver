namespace TestProjectAnnur.Data.DTOs
{
    public class UserDTO
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string? Password { get; set; }
        public int RoleId { get; set; }
    }

    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

namespace TestProjectAnnur.Data.DTOs
{
    public class RegisterDTO
    {
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }

    public class LoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponseDTO
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
    }
}

using TestProjectAnnur.Data.DTOs;
using TestProjectAnnur.Data.Models;
using TestProjectAnnur.Repositories;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using TestProjectAnnur.Data;
using Microsoft.EntityFrameworkCore;

namespace TestProjectAnnur.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthService(IUserService userService, IUserRepository userRepository, IConfiguration configuration, ApplicationDbContext context)
        {
            _userService = userService;
            _userRepository = userRepository;
            _configuration = configuration;
            _context = context;
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userRepository.GetUserByUsernameAsync(loginDTO.Username);

            if (user == null)
                throw new Exception("Username dan Password salah");

            if (!VerifyPassword(loginDTO.Password, user.Password))
                throw new Exception("Password salah");

            var token = await GenerateJwtToken(user);

            return new AuthResponseDTO
            {
                UserId = user.Id,
                Username = user.Username,
                Fullname = user.Fullname,
                Token = token
            };
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(registerDTO.Username);

            if (existingUserByUsername != null)
                throw new Exception("Username sudah digunakan");

            var userEntity = new User
            {
                Username = registerDTO.Username,
                Fullname = registerDTO.Fullname,
                Password = _userService.HashPassword(registerDTO.Password)
            };

            var createdUser = await _userRepository.CreateUserAsync(userEntity, registerDTO.RoleId);

            var token = await GenerateJwtToken(createdUser);

            return new AuthResponseDTO
            {
                UserId = createdUser.Id,
                Username = createdUser.Username,
                Fullname = createdUser.Fullname,
                Token = token
            };
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

            List<string> roleNames = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.Role.Name)
                .ToListAsync();


            List<Claim> claims = [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Fullname),
            ];
            claims.AddRange(roleNames.Select(r => new Claim(ClaimTypes.Role, r)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpirationHours"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }


        private bool VerifyPassword(string password, string storedHash) => _userService.HashPassword(password) == storedHash;
    }
}

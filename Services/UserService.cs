﻿using TestProjectAnnur.Data.DTOs;
using TestProjectAnnur.Data.Models;
using TestProjectAnnur.Repositories;
using System.Security.Cryptography;
using System.Text;
using TestProjectAnnur.Data;
using System.Data;

namespace TestProjectAnnur.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public UserService(IUserRepository userRepository, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<UserResponseDTO> CreateUserAsync(UserDTO userDto)
        {
            var userEntity = new User
            {
                Username = userDto.Username,
                Fullname = userDto.Fullname,
                Password = HashPassword(userDto.Password)
            };

            var createdUser = await _userRepository.CreateUserAsync(userEntity, userDto.RoleId);
            return MapToResponseDTO(createdUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(MapToResponseDTO);
        }

        public async Task<UserResponseDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
                return null;

            return MapToResponseDTO(user);
        }

        public async Task<UserResponseDTO> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);

            if (user == null)
                return null;

            return MapToResponseDTO(user);
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        public async Task<UserResponseDTO> UpdateUserAsync(int id, UserDTO userDto)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(id);

            if (existingUser == null)
                return null;

            existingUser.Username = userDto.Username;
            existingUser.Fullname = userDto.Fullname;
            if (userDto.Password != null)
            {
                existingUser.Password = HashPassword(userDto.Password);
            }
            existingUser.UpdatedAt = DateTime.UtcNow;

            var updatedUser = await _userRepository.UpdateUserAsync(existingUser);
            return MapToResponseDTO(updatedUser);
        }

        public async Task<SaveResponse> ProcessUserImport(List<UserDTO> excelData)
        {
            int baris = 0;
            DateTime currDate = DateTime.Now;
            using var transac = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in excelData)
                {
                    var user = new User
                    {
                        Username = item.Username,
                        Fullname = item.Fullname,
                        Password = HashPassword(item.Password),
                        CreatedAt = currDate,
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    var userRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = item.RoleId,
                    };
                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();
                }
                await transac.CommitAsync();
                return new SaveResponse(true, "Uploaded Successfully");
            } catch (Exception ex)
            {
                await transac.RollbackAsync();
                var message = ex.Message;
                var inex = ex.InnerException?.Message ?? "⚠";
                throw new Exception(message + " | " + inex + " - Error on your data at row " + baris);
            }
        } 

        private UserResponseDTO MapToResponseDTO(User user)
        {
            int role = _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.Role.Id)
                .FirstOrDefault();

            return new UserResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                Fullname = user.Fullname,
                RoleId = role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}

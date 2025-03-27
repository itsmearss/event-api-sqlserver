using Microsoft.Extensions.Logging;
using TestProjectAnnur.Data.DTOs;
using TestProjectAnnur.Data.Models;
using TestProjectAnnur.Repositories;

namespace TestProjectAnnur.Services
{
    public class RegisterEventService : IRegisterEventService
    {
        private readonly IRegisterEventRepository _reRepository;
        private readonly IFileService _fileService;

        public RegisterEventService(IRegisterEventRepository reRepository, IFileService fileService)
        {
            _reRepository = reRepository;
            _fileService = fileService;
        }

        public async Task<RegisterEventResponseDTO> CreateRegisterEventAsync(RegisterEventDTO registerEventDTO)
        {
            var registerEvent = new RegisterEvent
            {
                UserId = (int)registerEventDTO.UserId,
                EventId = registerEventDTO.EventId,
                IsAttend = AttendStatus.Attend,
            };

            var createdRegisterEvent = await _reRepository.CreateRegisterEventAsync(registerEvent);
            var response = MapToResponseDTO(createdRegisterEvent);
            return response;

        }

        public async Task<bool> DeleteRegisterEventAsync(int id)
        {
            var existingRegisterEvent = await _reRepository.GetRegisterEventByIdAsync(id);

            if (existingRegisterEvent == null)
                throw new Exception($"RegisterEvent dengan ID {id} tidak ditemukan");

            if (existingRegisterEvent.Certificate != null)
            {
                _fileService.DeleteFile(existingRegisterEvent.Certificate);
            }

            return await _reRepository.DeleteRegisterEventAsync(id);
        }

        public async Task<IEnumerable<RegisterEventResponseDTO>> GetAllRegisterEventsAsync()
        {
            var registerEvents = await _reRepository.GetAllRegisterEventsAsync();
            return registerEvents.Select(MapToResponseDTO);
        }

        public async Task<RegisterEventResponseDTO> GetRegisterEventAsync(int userId, int eventId)
        {
            var registerEvent = await _reRepository.GetRegisterEventAsync(userId, eventId);

            if (registerEvent == null)
                return null;

            return MapToResponseDTO(registerEvent);
        }

        public async Task<RegisterEventResponseDTO> GetRegisterEventById(int id)
        {
            
            var registerEvent = await _reRepository.GetRegisterEventByIdAsync(id);

            if (registerEvent == null)
                return null;

            return MapToResponseDTO(registerEvent);
        }

        public async Task<IEnumerable<RegisterEventResponseDTO>> GetRegisterEventsByEventIdAsync(int id)
        {
            var registerEvents = await _reRepository.GetRegisterEventByEventIdAsync(id);

            if(registerEvents == null)
            {
                return null;
            }

            return registerEvents.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<RegisterEventResponseDTO>> GetRegisterEventsByUserIdAsync(int id)
        {
            var registerEvents = await _reRepository.GetRegisterEventsByUserIdAsync(id);

            if (registerEvents == null)
            {
                return null;
            }

            return registerEvents.Select(MapToResponseDTO);
        }

        public async Task<RegisterEventResponseDTO> UpdateRegisterEventAsync(int id, RegisterEventDTO registerEventDTO)
        {
            var existingRegisterEvent = await _reRepository.GetRegisterEventByIdAsync(id);

            if (existingRegisterEvent == null)
                return null;

            if(existingRegisterEvent.Certificate != null)
            {
                string oldCertificate = existingRegisterEvent.Certificate;
                registerEventDTO.CertificateName = oldCertificate;
            }

            string[] allowedFileExtensions = [".pdf"];

            if(registerEventDTO.Certificate != null)
            {
                if (registerEventDTO.Certificate.Length > 5 * 1024 * 1024)
                {
                    throw new Exception("Certificate size should not exceed 5 MB");
                }
                string createdCertificateName = await _fileService.SaveFileAsync(registerEventDTO.Certificate, allowedFileExtensions);
                registerEventDTO.CertificateName = createdCertificateName;

                if (existingRegisterEvent.Certificate != null)
                {
                    _fileService.DeleteFile(existingRegisterEvent.Certificate);
                }


                existingRegisterEvent.Certificate = createdCertificateName;
            }

            existingRegisterEvent.UpdatedAt = DateTime.UtcNow;
            existingRegisterEvent.IsAttend = registerEventDTO.IsAttend;

            var updatedRegisterEvent = await _reRepository.UpdateRegisterEventAsync(existingRegisterEvent);
            return MapToResponseDTO(updatedRegisterEvent);
        }

        private RegisterEventResponseDTO MapToResponseDTO(RegisterEvent registerEvent)
        {
            return new RegisterEventResponseDTO
            {
                Id = registerEvent.Id,
                UserId = registerEvent.UserId,
                EventId = registerEvent.EventId,
                IsAttend = registerEvent.IsAttend,
                Certificate = registerEvent.Certificate,
                CreatedAt = registerEvent.CreatedAt,
                UpdatedAt = registerEvent.UpdatedAt
            };
        }
    }
}

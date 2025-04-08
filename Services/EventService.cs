using System.Threading.Tasks;
using TestProjectAnnur.Data;
using TestProjectAnnur.Data.DTOs;
using TestProjectAnnur.Data.Models;
using TestProjectAnnur.Repositories;

namespace TestProjectAnnur.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICategoryService _categoryService;
        private readonly IFileService _fileService;

        public EventService(IEventRepository eventRepository, ICategoryService categoryService, IFileService fileService)
        {
            _eventRepository = eventRepository;
            _categoryService = categoryService;
            _fileService = fileService;
        }

        public async Task<EventResponseDTO> CreateEventAsync(EventDTO eventDTO)
        {
            if(eventDTO.Flyer.Length > 1 * 1024 * 1024)
            {
                throw new Exception("Flyer size should not exceed 1 MB");
            }

            if (eventDTO.Flyer.Length > 1 * 1024 * 1024)
            {
                throw new Exception("Cover size should not exceed 1 MB");
            }

            string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];
            string createdFlyerName = await _fileService.SaveFileAsync(eventDTO.Flyer, allowedFileExtentions);
            string createdCoverName = await _fileService.SaveFileAsync(eventDTO.Cover, allowedFileExtentions);

            var eventEntity = new Event
            {
                Title = eventDTO.Title,
                Description = eventDTO.Description,
                CategoryId = eventDTO.CategoryId,
                Date = eventDTO.Date,
                Time = eventDTO.Time,
                Location = eventDTO.Location,
                MaxAttendees = eventDTO.MaxAttendees,
                Status = eventDTO.Status,
                Flyer = createdFlyerName,
                Cover = createdCoverName,
                CreatedAt = DateTime.UtcNow
            };

            var createdEvent = await _eventRepository.CreateEventAsync(eventEntity);
            return await MapToResponse(createdEvent);
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var existingEvent = await _eventRepository.GetEventByIdAsync(id);

            if(existingEvent == null)
            {
                throw new Exception($"Event dengan ID {id} tidak ditemukan");
            }

            _fileService.DeleteFile(existingEvent.Flyer);
            _fileService.DeleteFile(existingEvent.Cover);

            return await _eventRepository.DeleteEventAsync(id);
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            Console.WriteLine(events.Select(MapToResponse));
            return events;
        }

        public async Task<EventResponseDTO> GetEventByIdAsync(int id)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(id);

            if (eventEntity == null)
                return null;

            return await MapToResponse(eventEntity);
        }

        public async Task<EventResponseDTO> UpdateEventAsync(int id, EventDTO eventDTO)
        {
            var existingEvent = await _eventRepository.GetEventByIdAsync(id);

            if (existingEvent == null)
                return null;

            string oldFlyerImage = existingEvent.Flyer;
            eventDTO.FlyerName = oldFlyerImage;
            string oldCoverImage = existingEvent.Cover;
            eventDTO.CoverName = oldCoverImage;

            string[] allowedFileExtensions = [".jpg", ".jpeg", ".png"];

            if (eventDTO.Flyer != null)
            {
                if(eventDTO.Flyer.Length > 1 * 1024 * 1024)
                {
                    throw new Exception("Flyer size should not exceed 1 MB");
                }
                string createdFlyerName = await _fileService.SaveFileAsync(eventDTO.Flyer, allowedFileExtensions);
                eventDTO.FlyerName = createdFlyerName;
                _fileService.DeleteFile(existingEvent.Flyer);
            }

            if(eventDTO.Cover != null)
            {
                if(eventDTO.Cover.Length > 1 * 1024 * 1024)
                {
                    throw new Exception("Cover size should not exceed 1 MB");
                }
                string createdCoverName = await _fileService.SaveFileAsync(eventDTO.Cover, allowedFileExtensions);
                eventDTO.CoverName = createdCoverName;
                _fileService.DeleteFile(existingEvent.Cover);
            }


            existingEvent.Title = eventDTO.Title;
            existingEvent.Description = eventDTO.Description;
            existingEvent.CategoryId = eventDTO.CategoryId;
            existingEvent.Date = eventDTO.Date;
            existingEvent.Time = eventDTO.Time;
            existingEvent.Location = eventDTO.Location;
            existingEvent.MaxAttendees = eventDTO.MaxAttendees;
            existingEvent.Status = eventDTO.Status;
            existingEvent.Flyer = eventDTO.FlyerName;
            existingEvent.Cover = eventDTO.CoverName;
            existingEvent.UpdatedAt = DateTime.UtcNow;

            var updatedEvent = await _eventRepository.UpdateEventAsync(existingEvent);
            return await MapToResponse(updatedEvent);
        }

        public async Task<List<ExportEvent>> GetDataExportEvent()
        {
            var events = await _eventRepository.GetAllEventsAsync();

            var data = events.Select(e => new ExportEvent
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                CategoryId = e.CategoryId,
                Date = e.Date,
                Time = e.Time,
                Location = e.Location,
                MaxAttendees = e.MaxAttendees,
                Status = e.Status,
                Flyer = e.Flyer,
                Cover = e.Cover,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();

            return data;
        }

        public async Task<EventResponseDTO> MapToResponse(Event eventEntity)
        {
            return new EventResponseDTO
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                Date = eventEntity.Date,
                Time = eventEntity.Time,
                CategoryId = eventEntity.CategoryId,
                Location = eventEntity.Location,
                MaxAttendees = eventEntity.MaxAttendees,
                Status = eventEntity.Status,
                Flyer = eventEntity.Flyer,
                Cover = eventEntity.Cover,
                CreatedAt = eventEntity.CreatedAt,
                UpdatedAt = eventEntity.UpdatedAt
            };
        }
    }
}

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

        public EventService(IEventRepository eventRepository, ICategoryService categoryService)
        {
            _eventRepository = eventRepository;
            _categoryService = categoryService;
        }

        public async Task<EventResponseDTO> CreateEventAsync(EventDTO eventDTO)
        {
            var eventEntity = new Event
            {
                Title = eventDTO.Title,
                Description = eventDTO.Description,
                CategoryId = eventDTO.CategoryId,
                StartTime = eventDTO.StartTime,
                EndTime = eventDTO.EndTime,
                Location = eventDTO.Location,
                MaxAttendees = eventDTO.MaxAttendees,
                Status = eventDTO.Status,
                Flyer = eventDTO.Flyer,
                Cover = eventDTO.Cover,
                CreatedAt = DateTime.UtcNow
            };

            var createdEvent = await _eventRepository.CreateEventAsync(eventEntity);
            return await MapToResponse(createdEvent);
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            return await _eventRepository.DeleteEventAsync(id);
        }

        public async Task<IEnumerable<EventResponseDTO>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return (IEnumerable<EventResponseDTO>)events.Select(MapToResponse);
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

            existingEvent.Title = eventDTO.Title;
            existingEvent.Description = eventDTO.Description;
            existingEvent.CategoryId = eventDTO.CategoryId;
            existingEvent.StartTime = eventDTO.StartTime;
            existingEvent.EndTime = eventDTO.EndTime;
            existingEvent.Location = eventDTO.Location;
            existingEvent.MaxAttendees = eventDTO.MaxAttendees;
            existingEvent.Status = eventDTO.Status;
            existingEvent.Flyer = eventDTO.Flyer;
            existingEvent.Cover = eventDTO.Cover;
            existingEvent.UpdatedAt = DateTime.UtcNow;

            var updatedEvent = await _eventRepository.UpdateEventAsync(existingEvent);
            return await MapToResponse(updatedEvent);
        }

        public async Task<EventResponseDTO> MapToResponse(Event eventEntity)
        {
            var category = await _categoryService.GetCategoryByIdAsync(eventEntity.Id);
            return new EventResponseDTO
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                StartTime = eventEntity.StartTime,
                EndTime = eventEntity.EndTime,
                Category = category.Name,
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

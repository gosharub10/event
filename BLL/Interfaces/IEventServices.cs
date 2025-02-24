using BLL.DTO;

namespace BLL.Interfaces;

public interface IEventServices
{
    Task<EventDTO> GetEventById(int id);
    Task<EventDTO> GetEventByName(string name);
    Task AddEvent(EventDTO eventDto);
    Task UpdateEvent(EventDTO eventDto);
    Task DeleteEvent(int id); 
    
    Task<List<EventDTO>> GetAll(DateOnly? date, string? location, string? category);
    Task UploadImage(int id, MemoryStream memoryStream);
}
using BLL.DTO;

namespace BLL.Interfaces;

public interface IEventServices
{
    Task<EventDTO> GetEventById(int id, CancellationToken cancellationToken);
    Task<EventDTO> GetEventByName(string name, CancellationToken cancellationToken);
    Task AddEvent(EventNewDTO eventDto, CancellationToken cancellationToken);
    Task UpdateEvent(EventDTO eventDto, CancellationToken cancellationToken);
    Task DeleteEvent(int id, CancellationToken cancellationToken); 
    
    Task<List<EventDTO>> GetAll(QueryHelperDTO query, CancellationToken cancellationToken);
    Task UploadImage(int id, MemoryStream memoryStream, CancellationToken cancellationToken);
}
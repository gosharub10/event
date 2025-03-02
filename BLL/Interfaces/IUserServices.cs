using BLL.DTO;

namespace BLL.Interfaces;

public interface IUserServices
{
    Task<UserDTO> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<UserDTO>> GetByEventName(string eventName, CancellationToken cancellationToken);
    Task AddUserToEvent(int eventId, int userId, CancellationToken cancellationToken);
    Task RemoveUserFromEvent(int eventId, int userId, CancellationToken cancellationToken);
}
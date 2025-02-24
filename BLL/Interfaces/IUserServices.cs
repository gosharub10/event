using BLL.DTO;

namespace BLL.Interfaces;

public interface IUserServices
{
    Task<UserDTO> GetByIdAsync(int id);
    Task<List<UserDTO>> GetByEventName(string eventName);
    Task AddUserToEvent(int eventId, int userId);
    Task RemoveUserFromEvent(int eventId, int userId);
}
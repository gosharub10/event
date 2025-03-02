using DAL.Models;

namespace DAL.Interfaces;

public interface IEventParticipantsRepository: IRepository<EventParticipant>
{
    Task RemoveParticipant(int eventId, int userId, CancellationToken cancellationToken);
}
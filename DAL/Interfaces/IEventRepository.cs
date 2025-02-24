using DAL.Models;

namespace DAL.Interfaces;

public interface IEventRepository: IRepository<Event>
{
    Task<Event> GetEventByName(string name);
}
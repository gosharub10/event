using BLL.DTO;
using DAL.Models;
using Mapster;

namespace BLL.Mapper;

public class EventConfigMapper: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Event, EventDTO>()
            .Map(dest => dest.Participants, 
                src => src.EventParticipants != null 
                    ? src.EventParticipants.Adapt<List<UserDTO>>(config) 
                    : new List<UserDTO>());
    }
}
using BLL.DTO;
using DAL.Models;
using Mapster;

namespace BLL.Mapper;

public class EventParticipantsMapper: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<EventParticipant, UserDTO>()
            .Map(dest => dest.Id, src => src.User.Id)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.FirstName, src => src.User.FirstName)
            .Map(dest => dest.LastName, src => src.User.LastName)
            .Map(dest => dest.Birthday, src => src.User.Birthday)
            .Map(dest => dest.RegistrationDate, src => src.RegistrationDate);
    }
}
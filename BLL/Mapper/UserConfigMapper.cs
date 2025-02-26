using BLL.DTO;
using DAL.Models;
using Mapster;

namespace BLL.Mapper;

public class UserConfigMapper: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserRegistrationDTO, User>()
            .Map(dest => dest.PasswordHash, src => src.Password);
    }
}
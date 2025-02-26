using BLL.DTO;
using DAL.Helpers;
using Mapster;

namespace BLL.Mapper;

public class QueryHelperConfigMapper: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QueryHelperDTO, QueryHelper>();
    }
}
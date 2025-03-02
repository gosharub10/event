using BLL.DTO;
using DAL.Models;

namespace BLL.Interfaces;

public interface IAuthServices
{
    Task Registration(UserRegistrationDTO user, CancellationToken cancellationToken);
    Task<TokenDTO> Login(LoginDTO loginDto, CancellationToken cancellationToken);
    Task<TokenDTO> RefreshToken(string tokenDto, CancellationToken cancellationToken);
}
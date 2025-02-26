using BLL.DTO;
using DAL.Models;

namespace BLL.Interfaces;

public interface IAuthServices
{
    Task Registration(UserRegistrationDTO user);
    Task<TokenDTO> Login(LoginDTO loginDto);
    Task<TokenDTO> RefreshToken(TokenDTO tokenDto);
}
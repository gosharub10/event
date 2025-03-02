using DAL.Models;

namespace DAL.Interfaces;

public interface IUserRepository: IRepository<User>
{
    Task<User> Login(string email, string password, CancellationToken cancellationToken);
    Task<List<string>> GetRoles(User user, CancellationToken cancellationToken);
}
using DAL.Context;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.PostgresRepository;

internal class UserPostgres: IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ApplicationDbContext _context;

    public UserPostgres(UserManager<User> userManager, SignInManager<User> signInManager,ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    public async Task Add(User entity, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;
        
        entity.UserName = entity.Email;
        await _userManager.CreateAsync(entity, entity.PasswordHash!);
        
        await _userManager.AddToRoleAsync(entity,"user");
    }

    public Task Update(User entity, CancellationToken cancellationToken)
    {
        _context.Update(entity);
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task Delete(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<User>> GetAll(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetById(int id, CancellationToken cancellationToken)
    {
        return (await _context.Users
            .Include(u => u.EventParticipants)
            .ThenInclude(e => e.Event)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken))!;
    }
    
    public async Task<User> Login(string email, string password, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return null!;
        
        var user = await _userManager.FindByEmailAsync(email);
        await _signInManager.CheckPasswordSignInAsync(user, password, false);

        return user;
    }

    public async Task<List<string>> GetRoles(User user, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return null!;
        return (List<string>)await _userManager.GetRolesAsync(user);
    }
}
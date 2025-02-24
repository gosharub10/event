using DAL.Context;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.PostgresRepository;

internal class UserPostgres: IUserRepository
{
    private readonly UserManager<User> _userManager;
    
    private readonly ApplicationDbContext _context;

    public UserPostgres(UserManager<User> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task Add(User entity)
    {
        await _userManager.CreateAsync(entity);
    }

    public Task Update(User entity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<User>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetById(int id)
    {
        return await _context.Users
            .Include(u => u.EventParticipants)
            .ThenInclude(e => e.Event)
            .FirstOrDefaultAsync(u => u.Id == id) ?? throw new NullReferenceException();
    }
}
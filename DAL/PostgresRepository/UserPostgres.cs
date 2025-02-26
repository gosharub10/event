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

    public async Task Add(User entity)
    {
        entity.UserName = entity.Email;
        var result = await _userManager.CreateAsync(entity, entity.PasswordHash!);

        if (!result.Succeeded)
            throw new Exception("user creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        
        var role = await _userManager.AddToRoleAsync(entity,"user");
        if (!result.Succeeded)
        {
            throw new Exception("cannot create role: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    public Task Update(User entity)
    {
        _context.Update(entity);
        return _context.SaveChangesAsync();
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
    
    public async Task<User> Login(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new Exception("invalid password or email");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
        {
            throw new Exception("invalid password or email");
        }

        return user;
    }

    public async Task<List<string>> GetRoles(User user)
    {
        return (List<string>)await _userManager.GetRolesAsync(user);
    }
}
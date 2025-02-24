using DAL.Context;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.PostgresRepository;

internal class EventPostgres: IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventPostgres(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task Add(Event entity)
    {
        await _context.Events.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Event entity)
    {
        _context.Events.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var entity = await _context.Events.FindAsync(id);
        if (entity != null)
        {
            _context.Events.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<List<Event>> GetAll()
    {
        return await _context.Events
            .Include(e => e.EventParticipants)
            .ThenInclude(u => u.User) 
            .ToListAsync();
    }

    public async Task<Event> GetById(int id)
    {
        return await _context.Events
            .Include(e => e.EventParticipants)
            .ThenInclude(u => u.User) 
            .FirstOrDefaultAsync(e => e.Id == id) ?? throw new ArgumentNullException(id.ToString());
    }

    public async Task<Event> GetEventByName(string name)
    {
        return await _context.Events
            .Include(e => e.EventParticipants)
            .ThenInclude(u => u.User)
            .FirstOrDefaultAsync(e => EF.Functions.ILike(e.Title.ToLower(), name.ToLower())) ?? throw new ArgumentNullException(name);
    }
}
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
    public async Task Add(Event entity, CancellationToken cancellationToken)
    {
        await _context.Events.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Event entity, CancellationToken cancellationToken)
    {
        _context.Events.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Events.FindAsync(id, cancellationToken);
        if (entity != null)
        {
            _context.Events.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
    public async Task<List<Event>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Events
            .Include(e => e.EventParticipants)
            .ThenInclude(u => u.User) 
            .ToListAsync(cancellationToken);
    }

    public async Task<Event> GetById(int id, CancellationToken cancellationToken)
    {
        return (await _context.Events
            .Include(e => e.EventParticipants)
            .ThenInclude(u => u.User) 
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken))!;
    }

    public async Task<Event> GetEventByName(string name, CancellationToken cancellationToken)
    {
        return (await _context.Events
            .Include(e => e.EventParticipants)
            .ThenInclude(u => u.User)
            .FirstOrDefaultAsync(e => EF.Functions.ILike(e.Title.ToLower(), name.ToLower()), cancellationToken))!;
    }
}
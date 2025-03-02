using DAL.Context;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.PostgresRepository;

internal class EventParticipantsPostgres: IEventParticipantsRepository
{
    private readonly ApplicationDbContext _context;

    public EventParticipantsPostgres(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Add(EventParticipant entity, CancellationToken cancellationToken)
    {
        
        await _context.EventParticipants.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task Update(EventParticipant entity, CancellationToken cancellationToken)
    {
        return null!;
    }

    public Task Delete(int id, CancellationToken cancellationToken)
    {
        return null!;
    }

    public async Task<List<EventParticipant>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.EventParticipants
            .Include(e => e.Event)
            .Include(u => u.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<EventParticipant> GetById(int id, CancellationToken cancellationToken)
    {
        return (await _context.EventParticipants
            .Include(u => u.User)
            .Include(e => e.Event)
            .FirstOrDefaultAsync(ep => ep.EventId == id, cancellationToken))!;
    }

    public async Task RemoveParticipant(int eventId, int userId, CancellationToken cancellationToken)
    {
        var participant = await _context.EventParticipants.FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.UserId == userId, cancellationToken);
        _context.EventParticipants.Remove(participant!);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
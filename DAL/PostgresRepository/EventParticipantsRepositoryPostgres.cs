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

    public async Task Add(EventParticipant entity)
    {
        await _context.EventParticipants.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public Task Update(EventParticipant entity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<EventParticipant>> GetAll()
    {
        return await _context.EventParticipants
            .Include(e => e.Event)
            .Include(u => u.User)
            .ToListAsync();
    }

    public async Task<EventParticipant> GetById(int id)
    {
        return await _context.EventParticipants
            .Include(u => u.User)
            .Include(e => e.Event)
            .FirstOrDefaultAsync(ep => ep.EventId == id) ?? throw new ArgumentNullException($"event with id {id} not found");
    }

    public async Task RemoveParticipant(int eventId, int userId)
    {
        var participant = await _context.EventParticipants.FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.UserId == userId);
        _context.EventParticipants.Remove(participant!);
        await _context.SaveChangesAsync();
    }
}
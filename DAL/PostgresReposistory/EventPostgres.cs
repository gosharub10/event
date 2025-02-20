using DAL.Context;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.PostgresReposistory;

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
            .Include(e => e.Participants)
            .ToListAsync();
    }

    public async Task<Event> GetById(int id)
    {
        return await _context.Events
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == id) ?? throw new InvalidOperationException();
    }
}
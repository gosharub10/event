using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context;

public class ApplicationDbContext: IdentityDbContext<User, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public ApplicationDbContext() 
    {
    }
    
    public DbSet<Event> Events { get; set; }
    public DbSet<EventParticipant> EventParticipants { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EventParticipant>()
            .HasKey(ep => new { ep.EventId, ep.UserId }); 

        modelBuilder.Entity<EventParticipant>()
            .HasOne(ep => ep.Event)          
            .WithMany(e => e.EventParticipants) 
            .HasForeignKey(ep => ep.EventId); 

        modelBuilder.Entity<EventParticipant>()
            .HasOne(ep => ep.User)          
            .WithMany(u => u.EventParticipants) 
            .HasForeignKey(ep => ep.UserId);  
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=event;Username=user;Password=password");
        }
    }
}
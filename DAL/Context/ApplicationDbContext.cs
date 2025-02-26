using System.Globalization;
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
        
        modelBuilder.Entity<IdentityRole<int>>(b =>
        {
            b.ToTable("AspNetRoles");
        });
        
        var admin = new IdentityRole<int>("admin");
        admin.NormalizedName = admin.Name!.ToUpper();
        admin.Id = 1;

        var client = new IdentityRole<int>("user");
        client.NormalizedName = client.Name!.ToUpper();
        client.Id = 2;
        
        modelBuilder.Entity<IdentityRole<int>>().HasData(admin, client);
        
        var adminUser = new User
        {
            Id = 1,
            UserName = "admin@admin.com",
            NormalizedUserName = "ADMIN@ADMIN.COM",
            Email = "admin@admin.com",
            NormalizedEmail = "ADMIN@ADMIN.COM",
            SecurityStamp = Guid.NewGuid().ToString("D"),
            FirstName = "Admin",
            LastName = "Admin",
            Birthday = DateOnly.FromDateTime(DateTime.ParseExact("2005-10-10", "yyyy-MM-dd", CultureInfo.InvariantCulture))
        };

        var hasher = new PasswordHasher<User>();
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "admin");

        modelBuilder.Entity<User>().HasData(adminUser);

        modelBuilder.Entity<IdentityUserRole<int>>().HasData(
            new IdentityUserRole<int>
            {
                RoleId = admin.Id,
                UserId = adminUser.Id
            }
        );
    }
}
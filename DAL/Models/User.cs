using Microsoft.AspNetCore.Identity;

namespace DAL.Models;

public class User : IdentityUser<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly Birthday { get; set; }
    public virtual ICollection<EventParticipant> EventParticipants { get; set; }
}
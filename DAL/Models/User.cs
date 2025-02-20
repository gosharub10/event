using DAL.Models;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime Birthday { get; set; }
    public virtual ICollection<Event> Events { get; set; }
}
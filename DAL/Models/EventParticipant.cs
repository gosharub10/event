namespace DAL.Models;

public class EventParticipant
{ 
    public int EventId { get; set; }
    public Event Event { get; set; }
    public int UserId { get; set; } 
    public User User { get; set; }
    public DateOnly RegistrationDate { get; set; }
}
namespace BLL.DTO;

public class EventDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateOnly EventDateTime { get; set; }
    public string Location { get; set; }
    public string Category { get; set; }
    public int MaxParticipants { get; set; }
    public List<UserDTO> Participants { get; set; }
    public byte[]? Image { get; set; }
}
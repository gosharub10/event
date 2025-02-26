namespace BLL.DTO;

public class EventNewDTO
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateOnly EventDateTime { get; set; }
    public string Location { get; set; }
    public string Category { get; set; }
    public int MaxParticipants { get; set; }
}
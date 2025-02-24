namespace BLL.DTO;

public class UserDTO
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly? RegistrationDate { get; set; }
    public DateOnly Birthday { get; set; }
}
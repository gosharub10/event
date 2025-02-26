namespace BLL.DTO;

public class QueryHelperDTO
{ 
    public string? Date { get; set; } 
    public string? Location { get; set; } 
    public string? Category { get; set; } 
    public int Page { get; set; } = 1; 
    public int PageSize { get; set; } = 10;
}
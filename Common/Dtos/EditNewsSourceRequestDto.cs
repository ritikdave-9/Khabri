using Common.Enums;

public class EditNewsSourceRequestDto
{
    public string Name { get; set; }
    public string BaseURL { get; set; }
    public string Token { get; set; }
    public NewsSourceStatus Status { get; set; } 
}

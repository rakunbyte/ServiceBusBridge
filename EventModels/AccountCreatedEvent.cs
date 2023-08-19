namespace EventModels;

public class AccountCreatedEvent
{
    public string? UserId { get; set; }
    public string? Source { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PartnerId { get; set; }
    public string? OriginalLoginMethod { get; set; }
}
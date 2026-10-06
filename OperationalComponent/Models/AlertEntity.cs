namespace CommandService.Models;

public class AlertEntity
{
    public string AlertId { get; set; } = string.Empty;
    public string? Source { get; set; } = string.Empty;
    public string? Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Priority { get; set; } = string.Empty;
    public string Classification { get; set; } = string.Empty;
    public decimal Lat { get; set; }
    public decimal Lon { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? Status { get; set; } = string.Empty;
}
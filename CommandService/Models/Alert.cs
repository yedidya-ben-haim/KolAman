using System.Text.Json.Serialization;

namespace CommandService.Models;

public class Alert
{
    [JsonPropertyName("alert_id")]
    public string? AlertId { get; set; } = string.Empty;
    
    [JsonPropertyName("source")]
    public string? Source { get; set; } = string.Empty;
    
    [JsonPropertyName("title")]
    public string? Title { get; set; } = string.Empty;
    
    [JsonPropertyName("content")]
    public string? Content { get; set; } = string.Empty;
    
    [JsonPropertyName("priority")]
    public string? Priority { get; set; } = string.Empty;
    
    [JsonPropertyName("classification")]
    public string? Classification { get; set; } = string.Empty;
    
    [JsonPropertyName("lat")]
    public string? Lat { get; set; }
    
    [JsonPropertyName("lon")]
    public string? Lon { get; set; }
    
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; } = string.Empty;
}
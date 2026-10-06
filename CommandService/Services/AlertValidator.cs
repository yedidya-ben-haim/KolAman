using CommandService.Models;

namespace CommandService.Services;

public class AlertValidator
{
    public bool ValidateAlert(Alert alert)
    {
        if (string.IsNullOrWhiteSpace(alert.AlertId))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(alert.Content))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(alert.Lat))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(alert.Lon))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(alert.Timestamp))
        {
            return false;
        }
        
        if (!double.TryParse(alert.Lat, out double lat)|| lat > 90 || lat < -90)
        {
            return false;
        }
        
        if (!double.TryParse(alert.Lon, out double lon)|| lon > 90 || lon < -90)
        {
            return false;
        }

        if (!DateTime.TryParse(alert.Timestamp, out _))
        {
            return false;
        }

        return true;
    }
}
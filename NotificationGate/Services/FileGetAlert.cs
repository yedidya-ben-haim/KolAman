using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using NotificationGate.Models;

namespace NotificationGate.Services;

public class FileGetAlert
{
    public Alert? GetAlert(string fileReadyPath)
    {
        var directory = Path.GetDirectoryName(fileReadyPath);
        var fileName = Path.GetFileNameWithoutExtension(fileReadyPath);

        var jsonPath = Path.Combine(directory, $"{fileName}.json");

        if (!File.Exists(jsonPath))
        {
            throw new FileNotFoundException("file {filePath} not exit", jsonPath);
        }
        try
        {
            var jsonContext = File.ReadAllText(jsonPath);
            var alert = JsonSerializer.Deserialize<Alert>(jsonContext);
            return alert;
        }
        catch (JsonException e)
        {
            throw new JsonException($"faild to Deserialize alert {jsonPath}");
        }
        catch (Exception e)
        {
            throw;
        }
        
    }
}
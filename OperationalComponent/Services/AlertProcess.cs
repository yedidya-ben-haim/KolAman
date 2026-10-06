using CommandService.Models;

namespace OperationalComponent.Services;

public class AlertProcess
{
    public async Task<AlertEntity> Process(AlertEntity alert)
    {
        Random r = new Random();
        int rInt = r.Next(0, 100);
        alert.Status = "INPROGRESS";
        await Task.Delay(TimeSpan.FromSeconds(rInt));
        alert.Status = "DONE";
        return alert;
    }
}
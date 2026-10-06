using Microsoft.EntityFrameworkCore;
using OperationalComponent.Services;


namespace OperationalComponent;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly AlertProcess _alertProcess;


    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, AlertProcess alertProcess)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _alertProcess = alertProcess;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var alerts = await dbContext.Alerts
            .Where(a => a.Timestamp > DateTime.Now() - 1).ToListAsync();

        foreach (var alert in alerts)
        {
            await _alertProcess.Process(alert);
            _logger.LogInformation("Processed alert: {alertId}", alert.id);
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            

            await Task.Delay(1000, stoppingToken);
        }
    }
}
using System.Text.Json;
using NotificationGate.Models;
using NotificationGate.Services;

namespace NotificationGate;

public class Worker : BackgroundService
{
    private static ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private static KafkaService _kafkaService;
    private readonly FileSystemWatcher _fileSystemWatcher;
    private static FileGetAlert _fileGetAlert;

    public Worker(ILogger<Worker> logger,
        IConfiguration configuration,
        KafkaService kafkaService, FileGetAlert fileGetAlert)
    {
        _logger = logger;
        _configuration = configuration;
        _kafkaService = kafkaService;
        _fileGetAlert = fileGetAlert;

        var fileWatchPath = _configuration["Alert:AlertPath"];

        _fileSystemWatcher = CreateFileSystemWatcher(fileWatchPath);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker start Work");

            try
            {
                _fileSystemWatcher.EnableRaisingEvents = true;

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            finally
            {
                _fileSystemWatcher.EnableRaisingEvents = false;
            }
        }
    }

    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation($"Created: {e.FullPath}");

        try
        {
            var alert = _fileGetAlert.GetAlert(e.FullPath);
            _kafkaService.Send("alert-report", alert);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex.Message);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError("error: {e}", ex);
        }
        
        
    }

    private static void OnError(object sender, ErrorEventArgs e) =>
        PrintException(e.GetException());

    private static void PrintException(Exception? ex)
    {
        if (ex != null)
        {
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine("Stacktrace:");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine();
            PrintException(ex.InnerException);
        }
    }

    private FileSystemWatcher CreateFileSystemWatcher(string fileWatchPath)
    {
        var watcher = new FileSystemWatcher(fileWatchPath);
        watcher.InternalBufferSize = 65536;

        watcher.NotifyFilter = NotifyFilters.FileName;

        watcher.Created += OnCreated;
        watcher.Error += OnError;

        watcher.Filter = "*.ready";
        watcher.IncludeSubdirectories = true;

        Console.WriteLine("Press enter to exit.");
        return watcher;
    }
}
using NotificationGate;
using NotificationGate.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<KafkaService>();
builder.Services.AddSingleton<FileGetAlert>();

var host = builder.Build();
host.Run();
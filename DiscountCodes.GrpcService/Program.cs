using Serilog;
using DiscountCodes.Core.Registrations;
using DiscountCodes.GrpcService.Services;
using DiscountCodes.Persistence.Data;
using DiscountCodes.Persistence.Registrations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
SetupSerilogLogger(builder);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDataServices(builder.Configuration);
builder.Services.AddCoreServices();
builder.Services.AddCryptoServices();

var app = builder.Build();

// Apply migrations automatically at app start
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DiscountDbContext>();
    db.Database.Migrate();
    db.Database.ExecuteSqlRaw("PRAGMA journal_mode = WAL;"); // improves write/read concurrency
}

// Configure the HTTP request pipeline.
app.MapGrpcService<DiscountService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

Log.Logger.Warning("DiscountCodes gRPC server is running.");
app.Run();


static void SetupSerilogLogger(WebApplicationBuilder builder)
{
    var serilogConfig = builder.Configuration.GetSection("Serilog");
    var enableConsoleLog = serilogConfig.GetValue<bool>("EnableConsoleLog");
    var fileSizeLimitBytes = serilogConfig.GetValue<long>("FileSizeLimitBytes", 10485760);
    var minimumLogLevel = serilogConfig.GetValue<string>("MinimumLogLevel", "Warning");
    var minimumLogLevelForConsole = serilogConfig.GetValue<string>("MinimumLogLevelForConsole", "Information");
    var pathAndFileNameBase = serilogConfig.GetValue<string>("PathAndFileNameBase", "logs/DiscountCode.json");
    var retainedFileLimitCount = serilogConfig.GetValue<int>("RetainedFileLimitCount", 50);

    var loggerConfig = new LoggerConfiguration()
        .MinimumLevel.Is(Enum.TryParse(minimumLogLevel, out Serilog.Events.LogEventLevel fileLevel) ? fileLevel : Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext();

    if (enableConsoleLog)
    {
        loggerConfig = loggerConfig.WriteTo.Console(
            restrictedToMinimumLevel: Enum.TryParse(minimumLogLevelForConsole, out Serilog.Events.LogEventLevel consoleLevel) ? consoleLevel : Serilog.Events.LogEventLevel.Information
        );
    }

    loggerConfig = loggerConfig.WriteTo.File(
        pathAndFileNameBase,
        fileSizeLimitBytes: fileSizeLimitBytes,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: retainedFileLimitCount,
        restrictedToMinimumLevel: Enum.TryParse(minimumLogLevel, out Serilog.Events.LogEventLevel fileLevel2) ? fileLevel2 : Serilog.Events.LogEventLevel.Warning,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1)
    );

    Log.Logger = loggerConfig.CreateLogger();
    builder.Host.UseSerilog();
}
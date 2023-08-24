using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestConsoleApp;
using z019.EodHistoricalData;

Console.WriteLine("Started");

var options = new EodHDClientOptions();

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.Bind(EodHDClientOptions.SectionName, options);

builder.Services
    .Configure<HostOptions>(options =>
        {
            options.ServicesStartConcurrently = true;
            options.ServicesStopConcurrently = false;
        })
    .AddLogging((logging) =>
        {
            logging.AddFilter("Microsoft", LogLevel.Warning);
            logging.AddFilter("System", LogLevel.Warning);
            logging.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
        })
    .AddEodHistoricalDataService(options)
    .AddHostedService<TestService>();

var app = builder.Build();
app.Run();

Console.WriteLine("Ended");
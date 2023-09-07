using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestConsoleApp;
using z019.EodHistoricalData;
using z019.Storage.SqlStorage;

Console.WriteLine("Started");

var options = new EodHDClientOptions();

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.Bind(EodHDClientOptions.SectionName, options);

var connectionString = new SqliteConnectionStringBuilder()
{
    DataSource = @"c:\Code\DB\z019.sqlite",
    Mode = SqliteOpenMode.ReadWriteCreate,
}.ToString();

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
    .AddPooledDbContextFactory<StorageDbContext>(options => options.UseSqlite(connectionString))
    .AddEodHistoricalDataService(options)
    .AddHostedService<Test2Service>();

var app = builder.Build();
app.Run();

Console.WriteLine("Ended");
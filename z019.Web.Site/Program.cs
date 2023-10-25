using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog.Events;
using Serilog;
using z019.Storage.SqlStorage;
using z019.Web.Site.Components;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Application Initializing");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    var connectionString = SqliteConnectionString(builder.Configuration);

    // Add services to the container.
    builder.Services.AddRazorComponents().AddInteractiveServerComponents();
    builder.Services.AddMudServices();
    builder.Services.AddPooledDbContextFactory<StorageDbContext>(options => options.UseSqlite(connectionString));
    builder.Services.AddDbContext<StorageDbContext>(options => options.UseSqlite(connectionString));

    Log.Information("Application Starting");

    var app = builder.Build();

    Log.Information("Application Started");

    DatabaseMigrate(app.Services);

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    // app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseAntiforgery();
    app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Application Ended\n");
    Log.CloseAndFlush();
}

static string SqliteConnectionString(ConfigurationManager configuration)
{
    var path = configuration["SqlitePath"];
    if (string.IsNullOrEmpty(path)) throw new NullReferenceException(nameof(path));

    var idx = path.LastIndexOf('/');
    if (idx > 2)
    {
        var leftPath = path[..idx];
        Directory.CreateDirectory(leftPath);
    }

    return new SqliteConnectionStringBuilder()
    {
        DataSource = path,
        Mode = SqliteOpenMode.ReadWriteCreate,
    }.ToString();
}

static void DatabaseMigrate(IServiceProvider services)
{
    using var scope = services.CreateScope();
    using var context = scope.ServiceProvider.GetService<StorageDbContext>();
    context?.Database.EnsureCreated();
}
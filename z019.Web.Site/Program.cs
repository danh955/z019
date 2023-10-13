using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using z019.Storage.SqlStorage;
using z019.Web.Site.Components;

var connectionString = new SqliteConnectionStringBuilder()
{
    DataSource = @"c:\Code\DB\z019.sqlite",
    Mode = SqliteOpenMode.ReadWriteCreate,
}.ToString();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddPooledDbContextFactory<StorageDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDbContext<StorageDbContext>(options => options.UseSqlite(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();

namespace z019.Storage.SqlStorage;

using Microsoft.EntityFrameworkCore;

public class StorageDbContext : DbContext
{
    public StorageDbContext(DbContextOptions<StorageDbContext> options) : base(options)
    {
    }

    public DbSet<Exchange> Exchanges { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StorageDbContext).Assembly);

        SetCaseInsensitiveSearchesForSQLite(modelBuilder);
    }

    // <!-- https://stackoverflow.com/questions/65422810/sqlite-ef-core-5-wont-let-me-do-case-insensitive-searches -->
    private static void SetCaseInsensitiveSearchesForSQLite(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("NOCASE");

        foreach (var property in modelBuilder.Model.GetEntityTypes()
                                                .SelectMany(t => t.GetProperties())
                                                .Where(p => p.ClrType == typeof(string)))
        {
            property.SetCollation("NOCASE");
        }
    }
}
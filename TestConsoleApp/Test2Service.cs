namespace TestConsoleApp;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using z019.EodHistoricalData;
using z019.Storage.SqlStorage;

public class Test2Service : BackgroundService
{
    private readonly EodHDClient client;
    private readonly IDbContextFactory<StorageDbContext> dbFactory;
    private readonly ILogger<Test2Service> logger;

    public Test2Service(EodHDClient client, IDbContextFactory<StorageDbContext> dbFactory, ILogger<Test2Service> logger)
    {
        this.client = client;
        this.dbFactory = dbFactory;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var sourceExchanges = await client.GetExchangesListAsync(cancellationToken);

        using var db = this.dbFactory.CreateDbContext();
        db.Database.EnsureCreated();

        var currentExchanges = await db.Exchanges.ToDictionaryAsync(e => e.Code, StringComparer.Ordinal, cancellationToken);

        List<Exchange> updateExchanges = new();
        List<Exchange> addExchanges = new();

        foreach (var item in sourceExchanges)
        {
            var found = currentExchanges.GetValueOrDefault(item.Code);
            if (found == null)
            {
                // Add
                addExchanges.Add(new Exchange()
                {
                    Name = item.Name,
                    Code = item.Code.ToUpper(),
                    OperatingMIC = item.OperatingMIC ?? string.Empty,
                    Country = item.Country,
                    Currency = item.Currency,
                    CountryISO2 = item.CountryISO2,
                    CountryISO3 = item.CountryISO3
                });
            }
            else
            {
                // Update
                if (found.Name != item.Name
                    || found.OperatingMIC != (item.OperatingMIC ?? string.Empty)
                    || found.Country != item.Country
                    || found.Currency != item.Currency
                    || found.CountryISO2 != item.CountryISO2
                    || found.CountryISO3 != item.CountryISO3)
                {
                    found.Name = item.Name;
                    found.OperatingMIC = item.OperatingMIC ?? string.Empty;
                    found.Country = item.Country;
                    found.Currency = item.Currency;
                    found.CountryISO2 = item.CountryISO2;
                    found.CountryISO3 = item.CountryISO3;
                    updateExchanges.Add(found);
                }
            }
        }

        if (updateExchanges.Count > 0)
        {
            db.Exchanges.UpdateRange(updateExchanges);
            await db.SaveChangesAsync(cancellationToken);
        }

        if (addExchanges.Count > 0)
        {
            db.Exchanges.AddRange(addExchanges);
            await db.SaveChangesAsync(cancellationToken);
        }

        logger.LogInformation("Exchanges Added: {added}  Updated: {updated}", addExchanges.Count, updateExchanges.Count);
    }
}
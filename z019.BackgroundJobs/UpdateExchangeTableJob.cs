namespace z019.BackgroundJobs;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using z019.EodHistoricalData;
using z019.Storage.SqlStorage;

/// <summary>
/// Update the exchange table job.
/// </summary>
/// <param name="client">EodHDClient.</param>
/// <param name="db">StorageDbContext.</param>
/// <param name="logger">ILogger.</param>
internal class UpdateExchangeTableJob(EodHDClient client, IDbContextFactory<StorageDbContext> dbFactory, ILogger<UpdateExchangeTableJob> logger)
{
    private readonly EodHDClient client = client;
    private readonly IDbContextFactory<StorageDbContext> dbFactory = dbFactory;
    private readonly ILogger<UpdateExchangeTableJob> logger = logger;

    /// <summary>
    /// The status changed on the Percentage or IsBusy.
    /// </summary>
    internal event Action? OnStatusChanged;

    /// <summary>
    /// Job is busy running.
    /// </summary>
    internal bool IsBusy { get; private set; } = false;

    /// <summary>
    /// The percentage of the job completed.
    /// </summary>
    internal int Percentage { get; private set; } = 0;

    /// <summary>
    /// Run the Exchange table update job.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>Task.</returns>
    internal async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            StatusChanged(0, true);

            var sourceExchanges = await client.GetExchangesListAsync(cancellationToken);

            StatusChanged(20);
            using var db = this.dbFactory.CreateDbContext();
            var currentExchanges = await db.Exchanges.ToDictionaryAsync(e => e.Code, StringComparer.Ordinal, cancellationToken);

            StatusChanged(40);
            List<Exchange> updateExchanges = [];
            List<Exchange> addExchanges = [];

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

            StatusChanged(60);
            if (updateExchanges.Count > 0)
            {
                db.Exchanges.UpdateRange(updateExchanges);
                await db.SaveChangesAsync(cancellationToken);
            }

            StatusChanged(80);
            if (addExchanges.Count > 0)
            {
                db.Exchanges.AddRange(addExchanges);
                await db.SaveChangesAsync(cancellationToken);
            }

            logger.LogInformation("Exchanges Added: {added}  Updated: {updated}", addExchanges.Count, updateExchanges.Count);
        }
        catch (Exception e)
        {
            logger.LogError(e, "{class}", nameof(UpdateExchangeTableJob));
        }
        finally
        {
            StatusChanged(100, false);
        }
    }

    private void StatusChanged(int percentage, bool? isBusy = null)
    {
        Percentage = percentage;
        IsBusy = isBusy ?? IsBusy;
        OnStatusChanged?.Invoke();
    }
}
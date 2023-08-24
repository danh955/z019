namespace z019.EodHistoricalData;

using System.Collections.Generic;
using System.Threading.Tasks;

public partial class EodHDClient
{
    private const string ExchangesListAction = @"exchanges-list";

    /// <summary>
    /// Get list of exchanges.
    /// </summary>
    /// <returns>List of exchanges.</returns>
    public async Task<List<EodHDExchangesList>> GetExchangesListAsync(
        CancellationToken cancellationToken = default)
    {
        var url = ApiUrlBuilder(ExchangesListAction);
        return await ExecuteJsonQueryAsync<EodHDExchangesList>(url, cancellationToken).ConfigureAwait(false);
    }

    public record EodHDExchangesList(string Name, string Code, string? OperatingMIC, string Country, string Currency, string CountryISO2, string CountryISO3);
}
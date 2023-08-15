namespace z019.EodHistoricalData;

using System.Collections.Generic;
using System.Threading.Tasks;
using CsvHelper.Configuration;

/// <summary>
/// GetExchangeSymbolList file.
/// <!-- https://eodhistoricaldata.com/financial-apis/exchanges-api-list-of-tickers-and-trading-hours -->
/// CSV file header names: Code,Name,Country,Exchange,Currency,Type,Isin
/// </summary>
public sealed partial class EodHDClient
{
    private const string ExchangeSymbolListAction = @"exchange-symbol-list";

    /// <summary>
    /// Get Exchange Symbol List
    /// </summary>
    /// <param name="exchangeCode">Exchange code.  Default is 'US'.</param>
    /// <returns>List of exchange symbols.</returns>
    public async Task<List<EodExchangeSymbol>> GetExchangeSymbolListAsync(
        string? exchangeCode = null,
        CancellationToken cancellationToken = default)
    {
        return await GetExchangeSymbolListAsync<EodExchangeSymbol>(exchangeCode, null, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get Exchange Symbol List
    /// </summary>
    /// <typeparam name="T">The data row class name to put in the list.</typeparam>
    /// <param name="exchangeCode">Exchange code.  Default is 'US'.</param>
    /// <param name="classMap">Maps class members to CSV fields.  See CsvHelper.</param>
    /// <returns>List of exchange symbols.</returns>
    public async Task<List<T>> GetExchangeSymbolListAsync<T>(
        string? exchangeCode = null,
        ClassMap<T>? classMap = null,
        CancellationToken cancellationToken = default)
    {
        var url = ApiUrlBuilder(ExchangeSymbolListAction,
            data: exchangeCode,
            csv: true);

        return await ExecuteQueryAsync<T>(url, classMap, cancellationToken).ConfigureAwait(false);
    }

    public record EodExchangeSymbol(string Code, string Name, string Country, string Exchange, string Currency, string Type, string Isin);
}
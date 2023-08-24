namespace z019.EodHistoricalData;

using System.Collections.Generic;
using System.Threading.Tasks;
using CsvHelper.Configuration;

/// <summary>
/// Bulk API end-of-day splits dividends
/// <!-- https://eodhistoricaldata.com/financial-apis/bulk-api-eod-splits-dividends -->
/// Code,Ex,Date,Open,High,Low,Close,Adjusted_close,Volume
/// </summary>
public partial class EodHDClient
{
    private const string EodBulkLastDayAction = @"eod-bulk-last-day";

    /// <summary>
    /// Get end of day bulk last day data.
    /// </summary>
    /// <param name="exchangeCode">Stock Exchange code.  Default is 'US'.</param>
    /// <param name="symbols">A comma delimited list of stock symbols to retrieve.  Default is all.</param>
    /// <param name="date">The date to collect the stock prices.  Default is todays date.</param>
    /// <returns>List of price data for each stock symbol on a selected day.</returns>
    public async Task<List<EodHDLastPrice>> GetEodBulkLastDayAsync(
        string? exchangeCode = null,
        string? symbols = null,
        DateOnly? date = null,
        CancellationToken cancellationToken = default)
    {
        return await GetEodBulkLastDayAsync<EodHDLastPrice>(exchangeCode, symbols, date, null, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get end of day bulk last day data.
    /// </summary>
    /// <typeparam name="T">The data row class name to put in the list.</typeparam>
    /// <param name="exchangeCode">Stock Exchange code.  Default is 'US'.</param>
    /// <param name="symbols">A comma delimited list of stock symbols to retrieve.  Default is all.</param>
    /// <param name="date">The date to collect the stock prices.  Default is todays date.</param>
    /// <param name="classMap">Maps class members to CSV fields.  See CsvHelper.</param>
    /// <returns>List of price data for each stock symbol on a selected day.</returns>
    public async Task<List<T>> GetEodBulkLastDayAsync<T>(
        string? exchangeCode = null,
        string? symbols = null,
        DateOnly? date = null,
        ClassMap<T>? classMap = null,
        CancellationToken cancellationToken = default)
    {
        exchangeCode ??= "US";
        var url = ApiUrlBuilder(EodBulkLastDayAction, data: exchangeCode, symbols: symbols, date: date, csv: true);

        return await ExecuteCsvQueryAsync<T>(url, classMap, cancellationToken).ConfigureAwait(false);
    }

    public record EodHDLastPrice(string Code, string Ex, DateOnly Date, double Open, double High, double Low, double Close, double Adjusted_close, long Volume);
}
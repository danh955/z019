namespace z019.EodHistoricalData;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CsvHelper.Configuration;

/// <summary>
/// End-Of-Day Historical Stock Market Data
/// <!-- https://eodhistoricaldata.com/financial-apis/api-for-historical-data-and-volumes -->
/// CSV file header names: Date,Open,High,Low,Close,Adjusted_close,Volume
/// </summary>
public partial class EodHDClient
{
    private const string EodAction = @"eod";

    /// <summary>
    /// Get end of day price data.
    /// </summary>
    /// <param name="symbol">Sock symbol code.</param>
    /// <param name="exchangeCode">Stock Exchange code.  Default is 'US'.</param>
    /// <param name="fromDate">From date to retrieve stock prices.</param>
    /// <param name="toDate">The last date to retrieve stock prices. If null, todays date.</param>
    /// <param name="period">Day, week, or month.</param>
    /// <returns>List of end of day price data.</returns>
    public async Task<List<EodHDPrice>> GetEodAsync(
        string symbol,
        string? exchangeCode = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DataPeriod? period = null,
        CancellationToken cancellationToken = default)
    {
        return await GetEodAsync<EodHDPrice>(symbol, exchangeCode, fromDate, toDate, period, null, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get end of day price data.
    /// </summary>
    /// <typeparam name="T">The data row class name to put in the list.</typeparam>
    /// <param name="symbol">Sock symbol code.</param>
    /// <param name="exchangeCode">Stock Exchange code.  Default is 'US'.</param>
    /// <param name="fromDate">From date to retrieve stock prices.</param>
    /// <param name="toDate">The last date to retrieve stock prices. If null, todays date.</param>
    /// <param name="period">Day, week, or month.</param>
    /// <param name="classMap">Maps class members to CSV fields.  See CsvHelper.</param>
    /// <returns>List of end of day price data.</returns>
    public async Task<List<T>> GetEodAsync<T>(
                                string symbol,
                                string? exchangeCode = null,
                                DateOnly? fromDate = null,
                                DateOnly? toDate = null,
                                DataPeriod? period = null,
                                ClassMap<T>? classMap = null,
                                CancellationToken cancellationToken = default)
    {
        exchangeCode ??= "US";
        var url = ApiUrlBuilder(EodAction,
                                data: $"{symbol}.{exchangeCode}",
                                fromDate: fromDate,
                                toDate: toDate,
                                period: period,
                                csv: true);

        return await ExecuteCsvQueryAsync<T>(url, classMap, cancellationToken).ConfigureAwait(false);
    }

    public record EodHDPrice(DateOnly Date, double Open, double High, double Low, double Close, double Adjusted_close, long Volume);
}
namespace z019.EodHistoricalData;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Polly;

public sealed partial class EodHDClient
{
    private const string ApiUrl = @"https://eodhistoricaldata.com/api/";

    private readonly EodHDClientOptions options;
    private readonly HttpClient httpClient;
    private readonly ILogger? logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">EodHDClientOptions.</param>
    /// <param name="logger">ILogger.</param>
    /// <param name="httpClient">Optional HttpClient.</param>
    /// <exception cref="NullReferenceException">When EodHDClientOptions.ApiToken is null or empty.</exception>
    public EodHDClient(EodHDClientOptions options, ILogger? logger = null, HttpClient? httpClient = null)
    {
        if (string.IsNullOrWhiteSpace(options.ApiToken)) throw new NullReferenceException(nameof(options.ApiToken));
        this.options = options;
        this.logger = logger;

        if (httpClient == null)
        {
            var myHandler = new HttpClientHandler
            {
                DefaultProxyCredentials = CredentialCache.DefaultCredentials
            };

            this.httpClient = new HttpClient(myHandler);
        }
        else
        {
            // Used for mocking the HttpClient in a test.
            this.httpClient = httpClient;
        }
    }

    /// <summary>
    /// This executes the CSV query and returns the item list.
    /// </summary>
    /// <typeparam name="T">The class type to return.</typeparam>
    /// <param name="uri">The URL of the website to get the data.</param>
    /// <param name="classMap">The CSV Helper class map on how to populate the return class type.</param>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>A list of items.</returns>
    /// <exception cref="HttpRequestException"></exception>
    private async Task<List<T>> ExecuteCsvQueryAsync<T>(string uri, ClassMap<T>? classMap, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync<T>(uri, (response) => GetCsvFromResponseAsync<T>(response, classMap, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// This executes the JSON query and returns the item list.
    /// </summary>
    /// <typeparam name="T">The class type to return.</typeparam>
    /// <param name="uri">The URL of the website to get the data.</param>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>A list of items.</returns>
    /// <exception cref="HttpRequestException"></exception>
    private async Task<List<T>> ExecuteJsonQueryAsync<T>(string uri, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync<T>(uri, (response) => GetJsonFromResponseAsync<T>(response, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    private async Task<List<T>> ExecuteQueryAsync<T>(string uri, Func<HttpResponseMessage, Task<List<T>>> GetFromResponseAsync, CancellationToken cancellationToken)
    {
        this.logger?.LogDebug("httpClient.GetAsync {uri}", options.ApiToken == null ? "TokenMissing" : uri.Replace(options.ApiToken, "TokenRemoved"));

        Polly.Retry.AsyncRetryPolicy<HttpResponseMessage> httpRetryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => r.StatusCode == (HttpStatusCode)429)
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(60),
                TimeSpan.FromSeconds(90)
            },
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                //included for debug to see what the response is
                this.logger?.LogDebug("retryAttempt = {retryAttempt}, Result.StatusCode = {StatusCode}", retryAttempt, outcome.Result.StatusCode);
            });

        var response = await httpRetryPolicy.ExecuteAndCaptureAsync(async () => await this.httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

        if (response.Outcome == OutcomeType.Successful)
        {
            if (response.Result.IsSuccessStatusCode)
                return await GetFromResponseAsync(response.Result).ConfigureAwait(false);

            string message = $"There was an error while executing the HTTP query. Reason: {response.Result.ReasonPhrase}";
            this.logger?.LogDebug("Result: {reason}", message);
            throw new HttpRequestException(message);
        }
        else
        {
            var reason = response.FinalHandledResult != null ? response.FinalHandledResult.ReasonPhrase : response.FinalException.Message;
            string message = $"There was an error while executing the HTTP query. Reason: {reason}";
            this.logger?.LogDebug("Result: {reason}", message);
            throw new HttpRequestException(message);
        }
    }

    private async Task<List<T>> GetCsvFromResponseAsync<T>(HttpResponseMessage response, ClassMap<T>? classMap, CancellationToken cancellationToken)
    {
        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.TypeConverterCache.RemoveConverter<long>();
        csv.Context.TypeConverterCache.AddConverter<long>(new LongTypeConverter(logger));
        if (classMap != null) csv.Context.RegisterClassMap(classMap);
        return csv.GetRecords<T>().ToList();
    }

    private static async Task<List<T>> GetJsonFromResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        return await JsonSerializer.DeserializeAsync<List<T>>(stream, cancellationToken: cancellationToken).ConfigureAwait(false) ?? new();
    }

    private string ApiUrlBuilder(
        string action,
        string? data = null,
        DataPeriod? period = null,
        DataOrder? order = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateOnly? date = null,
        string? symbols = null,
        bool? csv = null)
    {
        StringBuilder builder = new(ApiUrl);
        builder.Append(action);

        if (data != null)
        {
            builder.Append('/');
            builder.Append(data);
        }

        char separator = '?';

        // Output format
        if (csv != null)
        {
            builder.Append(separator);
            builder.Append("fmt=");
            builder.Append(csv.Value ? "csv" : "json");
            separator = '&';
        }

        // Period: Day, Week, Month
        if (period != null)
        {
            builder.Append(separator);
            builder.Append("period=");
            builder.Append(period.Value switch
            {
                DataPeriod.Day => 'd',
                DataPeriod.Week => 'w',
                DataPeriod.Month => 'm',
                _ => throw new InvalidOperationException("Invalid Period")
            });
            separator = '&';
        }

        // Order: Ascending, Descending
        if (order != null)
        {
            builder.Append(separator);
            builder.Append("order=");
            builder.Append(order.Value switch
            {
                DataOrder.Ascending => 'a',
                DataOrder.Descending => 'd',
                _ => throw new InvalidOperationException("Invalid Period")
            });
            separator = '&';
        }

        // From date
        if (fromDate != null)
        {
            builder.Append(separator);
            builder.Append("from=");
            builder.Append(fromDate.Value.ToString("yyyy-MM-dd"));
            separator = '&';
        }

        // To date
        if (toDate != null)
        {
            builder.Append(separator);
            builder.Append("to=");
            builder.Append(toDate.Value.ToString("yyyy-MM-dd"));
            separator = '&';
        }

        // Just a date
        if (date != null)
        {
            builder.Append(separator);
            builder.Append("date=");
            builder.Append(date.Value.ToString("yyyy-MM-dd"));
            separator = '&';
        }

        if (symbols != null)
        {
            builder.Append(separator);
            builder.Append("symbols=");
            builder.Append(symbols);
            separator = '&';
        }

        builder.Append(separator);
        builder.Append("api_token=");
        builder.Append(this.options.ApiToken);

        return builder.ToString();
    }
}
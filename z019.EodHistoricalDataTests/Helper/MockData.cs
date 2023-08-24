namespace z018.EodHistoricalDataTests.Helper;

using System.Net;

internal static class MockData
{
    public static readonly Dictionary<string, HttpResponseMessage> Messages = new()
    {
        {
            "https://eodhistoricaldata.com/api/eod/msft.US?fmt=csv&api_token=Test",
            new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
@"
Date,Open,High,Low,Close,Adjusted_close,Volume
2023-05-23,288.15,288.5,285.44,286.37,284.8579,2943900.99
2023-05-24,286.45,287.12,284.58,285.92,284.4103,2262100
2023-05-25,286.79,286.91,284.71,285.52,284.0124,2748100
2023-05-26,285.68,288.75,285.68,286.04,284.5297,2148200
2023-05-30,283.93,286.35,283.68,284.92,283.4156,3385200
2023-05-31,284.88,285.7,283.35,285.11,283.6046,3104900
"),
            }
        },
        {
            "https://eodhistoricaldata.com/api/exchange-symbol-list?fmt=csv&api_token=Test",
            new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
@"
Code,Name,Country,Exchange,Currency,Type,Isin
A,""Agilent Technologies Inc"",USA,NYSE,USD,""Common Stock"",US00846U1016
AA,""Alcoa Corp"",USA,NYSE,USD,""Common Stock"",US0138721065
AAA,""Listed Funds Trust - AAF First Priority CLO Bond ETF"",USA,""NYSE ARCA"",USD,ETF,US53656F6566
AAAAX,""DEUTSCHE REAL ASSETS FUND CLASS A"",USA,NMFQS,USD,FUND,
AAACX,""A3 Alternative Credit Fund"",USA,NASDAQ,USD,FUND,
AAAEX,""Virtus AllianzGI Health Sciences Fund Class P"",USA,NMFQS,USD,FUND,
AAAFX,""American Century One Choice Blend+ 2015 Portfolio Investor Class"",USA,NMFQS,USD,FUND,
AAAGX,""THRIVENT LARGE CAP GROWTH FUND CLASS A"",USA,NMFQS,USD,FUND,US8858821007
"),
            }
        },
        {
            "https://eodhistoricaldata.com/api/eod-bulk-last-day/US?fmt=csv&api_token=Test",
            new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
@"
Code,Ex,Date,Open,High,Low,Close,Adjusted_close,Volume
A,US,2023-06-01,116.97,117.635,115,116.26,116.26,3965447
AA,US,2023-06-01,32,33.04,31.5201,32.7,32.7,4725340
AAA,US,2023-06-01,24.441,24.51,24.441,24.48,24.48,1723
AAACX,US,2023-06-01,6.43,6.43,6.43,6.43,6.43,0
AAAFX,US,2023-06-01,9.16,9.16,9.16,9.16,9.16,0
AAAHX,US,2023-06-01,9.16,9.16,9.16,9.16,9.16,0
AAAIX,US,2023-06-01,6.86,6.86,6.86,6.86,6.86,0
AAAJX,US,2023-06-01,9.14,9.14,9.14,9.14,9.14,0
"),
            }
        },
        {
            "https://eodhistoricaldata.com/api/exchanges-list?api_token=Test",
            new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
@"
[
  {
    ""Name"": ""USA Stocks"",
    ""Code"": ""US"",
    ""OperatingMIC"": ""XNAS, XNYS"",
    ""Country"": ""USA"",
    ""Currency"": ""USD"",
    ""CountryISO2"": ""US"",
    ""CountryISO3"": ""USA""
  },
  {
    ""Name"": ""London Exchange"",
    ""Code"": ""LSE"",
    ""OperatingMIC"": ""XLON"",
    ""Country"": ""UK"",
    ""Currency"": ""GBP"",
    ""CountryISO2"": ""GB"",
    ""CountryISO3"": ""GBR""
  },
  {
    ""Name"": ""NEO Exchange"",
    ""Code"": ""NEO"",
    ""OperatingMIC"": ""NEOE"",
    ""Country"": ""Canada"",
    ""Currency"": ""CAD"",
    ""CountryISO2"": ""CA"",
    ""CountryISO3"": ""CAN""
  },
  {
    ""Name"": ""TSX Venture Exchange"",
    ""Code"": ""V"",
    ""OperatingMIC"": ""XTSX"",
    ""Country"": ""Canada"",
    ""Currency"": ""CAD"",
    ""CountryISO2"": ""CA"",
    ""CountryISO3"": ""CAN""
  },
  {
    ""Name"": ""MICEX Moscow Russia"",
    ""Code"": ""MCX"",
    ""OperatingMIC"": null,
    ""Country"": ""Russia"",
    ""Currency"": ""RUB"",
    ""CountryISO2"": ""RU"",
    ""CountryISO3"": ""RUS""
  }
]
"),
            }
        },
    };
}
﻿namespace z019.EodHistoricalDataTests;

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using z018.EodHistoricalDataTests.Helper;

public class EodHDClient_GetExchangeSymbolListTest
{
    private readonly EodHDClient client;
    private readonly ILogger logger;

    public EodHDClient_GetExchangeSymbolListTest(ITestOutputHelper loggerHelper)
    {
        this.logger = loggerHelper.BuildLogger();
        this.client = new EodHDClient("Test", this.logger, new HttpClient(new MockHttpResponseMessage(MockData.Messages)));
    }

    [Fact]
    public async Task GetExchangeSymbolListTest()
    {
        var result = await client.GetExchangeSymbolListAsync();
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
    }
}
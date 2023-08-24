namespace z019.EodHistoricalDataTests;

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using z018.EodHistoricalDataTests.Helper;

public class EodHDClient_GetEodBulkLastDayTest
{
    private readonly EodHDClient client;
    private readonly ILogger logger;

    public EodHDClient_GetEodBulkLastDayTest(ITestOutputHelper loggerHelper)
    {
        this.logger = loggerHelper.BuildLogger();
        this.client = new EodHDClient(new EodHDClientOptions() { ApiToken = "Test" }, this.logger, new HttpClient(new MockHttpResponseMessage(MockData.Messages)));
    }

    [Fact]
    public async Task GetEodBulkLastDayTest()
    {
        var result = await client.GetEodBulkLastDayAsync();
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
    }
}
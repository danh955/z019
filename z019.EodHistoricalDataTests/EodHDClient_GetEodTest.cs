namespace z019.EodHistoricalDataTests;

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using z018.EodHistoricalDataTests.Helper;

public class EodHDClient_GetEodTest
{
    private readonly EodHDClient client;
    private readonly ILogger logger;

    public EodHDClient_GetEodTest(ITestOutputHelper loggerHelper)
    {
        this.logger = loggerHelper.BuildLogger();
        this.client = new EodHDClient("Test", this.logger, new HttpClient(new MockHttpResponseMessage(MockData.Messages)));
    }

    [Fact]
    public async Task GetEodTest()
    {
        var result = await client.GetEodAsync("msft");
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.True(result.First().Volume == 2943900);
    }
}
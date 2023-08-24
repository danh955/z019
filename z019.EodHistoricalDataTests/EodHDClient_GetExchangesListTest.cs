namespace z019.EodHistoricalDataTests
{
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;
    using z018.EodHistoricalDataTests.Helper;

    public class EodHDClient_GetExchangesListTest
    {
        private readonly EodHDClient client;
        private readonly ILogger logger;

        public EodHDClient_GetExchangesListTest(ITestOutputHelper loggerHelper)
        {
            this.logger = loggerHelper.BuildLogger();
            this.client = new EodHDClient(new EodHDClientOptions() { ApiToken = "Test" }, this.logger, new HttpClient(new MockHttpResponseMessage(MockData.Messages)));
        }

        [Fact]
        public async Task GetExchangesListTest()
        {
            var result = await client.GetExchangesListAsync();
            Assert.NotNull(result);
            Assert.True(result.Count > 0);
        }
    }
}
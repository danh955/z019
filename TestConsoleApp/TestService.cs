namespace TestConsoleApp
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using z019.EodHistoricalData;

    public class TestService : BackgroundService
    {
        private readonly EodHDClient client;
        private readonly ILogger<TestService> logger;

        public TestService(EodHDClient client, ILogger<TestService> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var a = await client.GetExchangesListAsync(stoppingToken);
        }
    }
}
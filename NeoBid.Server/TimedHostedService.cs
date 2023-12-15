using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NeoBid.Server.Data;

namespace NeoBid.Server
{
    public class TimedHostedService : BackgroundService
    {
        private readonly ILogger<TimedHostedService> _logger;
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public TimedHostedService(ILogger<TimedHostedService> logger, IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _logger = logger;
            _dbFactory = dbFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            await DoWork();

            using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await DoWork();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Timed Hosted Service is stopping.");
            }
        }

        private async Task DoWork()
        {
            try
            {
                using var context = await _dbFactory.CreateDbContextAsync();

                var currentTime = DateTime.Now;

                await context.Auctions.Where(x => x.EndsOn <= currentTime).ExecuteUpdateAsync(x => x.SetProperty(y => y.State, Data.Enums.AuctionState.Finished));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
        }
    }
}

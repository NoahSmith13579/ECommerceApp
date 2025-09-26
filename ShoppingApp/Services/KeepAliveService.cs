using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data;

namespace ShoppingApp.Services
// From https://learn.microsoft.com/en-us/answers/questions/1792711/sql-server-connection-timeout-after-idle
{
    public class KeepAliveService : BackgroundService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ILogger<KeepAliveService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);

        public KeepAliveService(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            ILogger<KeepAliveService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EF Core keep-alive service started. Interval: {Interval} minutes",
                _interval.TotalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var context = _contextFactory.CreateDbContext();

                    // Executes a lightweight query (works with retries if configured)
                    await context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken: stoppingToken);

                    _logger.LogInformation("EF Core keep-alive sent at {Time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "EF Core keep-alive failed at {Time}", DateTimeOffset.Now);
                }

                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // normal when the service is shutting down
                    break;
                }
            }

            _logger.LogInformation("EF Core keep-alive service stopped.");
        }
    }

}

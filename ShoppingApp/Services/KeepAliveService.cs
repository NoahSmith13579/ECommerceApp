using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ShoppingApp.Services
// From https://learn.microsoft.com/en-us/answers/questions/1792711/sql-server-connection-timeout-after-idle
{
    public class KeepAliveService : IHostedService, IDisposable

    {

        private Timer _timer;

        private readonly string _connectionString;
        private readonly ILogger<KeepAliveService> _logger;

        public KeepAliveService(IConfiguration configuration, ILogger<KeepAliveService> logger)

        {

            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)

        {
            _logger.LogInformation("EF Core keep-alive service started.");
            // Set up a timer to trigger the keep-alive query every 15 minutes

            _timer = new Timer(KeepAlive, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));

            return Task.CompletedTask;

        }

        private void KeepAlive(object state)

        {

            using (var connection = new SqlConnection(_connectionString))

            {

                try

                {

                    connection.Open();

                    using (var command = new SqlCommand("SELECT 1", connection))

                    {
                        _logger.LogInformation("EF Core keep-alive sent at {time}", DateTimeOffset.Now);
                        command.ExecuteScalar();

                    }
                }

                catch (Exception ex)

                {
                    // Handle exceptions as needed
                    _logger.LogError(ex, "EF Core keep-alive failed");

                }

            }

        }

        public Task StopAsync(CancellationToken cancellationToken)

        {

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;

        }

        public void Dispose()

        {

            _timer?.Dispose();

        }

    }

}

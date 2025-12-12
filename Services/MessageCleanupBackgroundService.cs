namespace MarutiTrainingPortal.Services
{
    /// <summary>
    /// Background service that automatically deletes contact messages older than 28 days
    /// Runs every day at 2:00 AM UTC
    /// </summary>
    public class MessageCleanupBackgroundService : BackgroundService
    {
        private readonly ILogger<MessageCleanupBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;

        public MessageCleanupBackgroundService(
            ILogger<MessageCleanupBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Message Cleanup Background Service started at {Time}", DateTime.UtcNow);

            // Calculate time until next 2:00 AM UTC
            var now = DateTime.UtcNow;
            var nextRun = now.Date.AddDays(1).AddHours(2); // 2 AM next day
            
            if (now.Hour >= 2)
            {
                nextRun = now.Date.AddDays(1).AddHours(2); // 2 AM next day
            }
            else
            {
                nextRun = now.Date.AddHours(2); // 2 AM today
            }

            var timeUntilNextRun = nextRun - now;
            _logger.LogInformation("Next message cleanup scheduled for {NextRun} UTC (in {TimeUntilNextRun})", nextRun, timeUntilNextRun);

            // Schedule the task to run daily at 2:00 AM UTC
            _timer = new Timer(
                callback: async _ => await CleanupExpiredMessages(stoppingToken),
                state: null,
                dueTime: timeUntilNextRun,
                period: TimeSpan.FromHours(24) // Run every 24 hours
            );

            await Task.CompletedTask;
        }

        private async Task CleanupExpiredMessages(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Starting automatic message cleanup at {Time}", DateTime.UtcNow);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var cleanupService = scope.ServiceProvider.GetRequiredService<IMessageCleanupService>();
                    
                    // Delete messages older than 28 days
                    var deletedCount = await cleanupService.DeleteExpiredMessagesAsync(daysOld: 28);
                    
                    _logger.LogInformation("Message cleanup completed. {DeletedCount} messages deleted at {Time}", 
                        deletedCount, DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during message cleanup: {ErrorMessage}", ex.Message);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            _logger.LogInformation("Message Cleanup Background Service stopped at {Time}", DateTime.UtcNow);
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}

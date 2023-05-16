using System.Runtime.InteropServices;

namespace PushListenerForLinux
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        SocketListener socketListener = new SocketListener();
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                socketListener.StartListening();
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
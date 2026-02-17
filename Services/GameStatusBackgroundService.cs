namespace StatusPainel.Services
{
    public class GameStatusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<GameStatusBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(10); // Aumentei para 10 minutos

        public GameStatusBackgroundService(IServiceProvider services, ILogger<GameStatusBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de atualização de status iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var gameStatusService = scope.ServiceProvider.GetRequiredService<GameStatusService>();

                        // Verifica status
                        await gameStatusService.CheckRealGameStatuses();

                        _logger.LogInformation("Status dos jogos atualizados em: {time}", DateTime.Now);
                    }

                    // Aguarda com tratamento de cancelamento
                    try
                    {
                        await Task.Delay(_checkInterval, stoppingToken);
                    }
                    catch (TaskCanceledException)
                    {
                        _logger.LogInformation("Serviço de background cancelado");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no serviço de background");

                    // Em caso de erro, aguarda 1 minuto antes de tentar novamente
                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                }
            }

            _logger.LogInformation("Serviço de atualização de status finalizado");
        }
    }
}
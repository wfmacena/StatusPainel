namespace StatusPainel.Services
{
    public class GameStatusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<GameStatusBackgroundService> _logger;

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

                        // Verifica status a cada 5 minutos
                        await gameStatusService.CheckRealGameStatuses();

                        _logger.LogInformation("Status dos jogos atualizados em: {time}", DateTime.Now);
                    }

                    // Aguarda 5 minutos antes da próxima verificação
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no serviço de background");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
}
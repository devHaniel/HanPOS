using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Data;
using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Api.Services.Implementations
{
    /// <summary>
    /// Background service que limpia refresh tokens expirados periódicamente.
    /// Se ejecuta cada 24 horas por defecto.
    /// </summary>
    public class RefreshTokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RefreshTokenCleanupService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(24);

        public RefreshTokenCleanupService(
            IServiceProvider serviceProvider,
            ILogger<RefreshTokenCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RefreshTokenCleanupService iniciado. Intervalo: {Interval}", _interval);

            // Ejecutar limpieza inicial al arrancar (con delay para no bloquear startup)
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
            await CleanupExpiredTokensAsync(stoppingToken);

            using var timer = new PeriodicTimer(_interval);

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                await CleanupExpiredTokensAsync(stoppingToken);
            }
        }

        private async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var now = DateTime.UtcNow;
                var expiredTokens = await context.HistorialRefreshTokens
                    .Where(t => t.FechaExpiracion <= now)
                    .ToListAsync(cancellationToken);

                if (expiredTokens.Count > 0)
                {
                    context.HistorialRefreshTokens.RemoveRange(expiredTokens);
                    var deleted = await context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Limpieza de refresh tokens: {Count} tokens expirados eliminados", deleted);
                }
                else
                {
                    _logger.LogDebug("Limpieza de refresh tokens: no hay tokens expirados");
                }
            }
            catch (OperationCanceledException)
            {
                // Normal al detener la aplicación
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante limpieza de refresh tokens expirados");
            }
        }
    }
}
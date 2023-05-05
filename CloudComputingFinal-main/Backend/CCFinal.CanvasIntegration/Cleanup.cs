using CCFinal.CanvasIntegration.Database;
using CCFinal.CanvasIntegration.Entities;
using Microsoft.EntityFrameworkCore;

public class Cleanup : BackgroundService {
    private readonly ILogger<Cleanup> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Cleanup(ILogger<Cleanup> logger, IServiceProvider serviceProvider) {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        await using var scope = _serviceProvider.CreateAsyncScope();

        while (!stoppingToken.IsCancellationRequested) {
            var context = scope.ServiceProvider.GetService<CanvasIntegrationDbContext>();
            if (context is null)
                continue;

            await CleanupLocks(context, TimeSpan.FromMinutes(10), stoppingToken);
            await CleanupEmptyTokens(context, stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }

    private async Task CleanupEmptyTokens(CanvasIntegrationDbContext context, CancellationToken stoppingToken) {
        List<UserInformation> users =
            await context.Information.Where(x => x.Token == "").ToListAsync(stoppingToken);
        if (users.Any()) {
            context.RemoveRange(users);
            await context.SaveChangesAsync(stoppingToken);
        }
    }

    private async Task CleanupLocks(CanvasIntegrationDbContext context,
        TimeSpan offset, CancellationToken stoppingToken = new()) {
        var time = DateTime.UtcNow.Add(offset);
        List<UserInformation> locks = await context.Information.Where(x => x.LockId != null && x.LockTime > time)
            .ToListAsync(stoppingToken);

        if (locks.Any())
            foreach (var userLock in locks) {
                userLock.LockId = null;
                userLock.LockTime = null;

                try {
                    await context.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation($"releasing lock on {userLock.UserID}");
                }
                catch (Exception ex) {
                    _logger.LogInformation($"unable to release lock on {userLock.UserID}");
                }
            }
    }
}
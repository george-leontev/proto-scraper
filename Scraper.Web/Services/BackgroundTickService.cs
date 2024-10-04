using Scraper.DataAccess;
using Scraper.Domain.Models;
using System.Threading;

public class BackgroundTickService : BackgroundService
{
    public IServiceProvider Services { get; }

    public BackgroundTickService(IServiceProvider services)
    {
        Services = services;
    }

    private void Callback(object? timerState)
    {
        Task.Run(async () =>
        {
            await DoProcessingAsync((CancellationToken)timerState!);
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new Timer(Callback,
            state: stoppingToken,
            dueTime: 10000,
            period: 10000
        );

        await Task.CompletedTask;
    }

    private async Task DoProcessingAsync(CancellationToken stoppingToken)
    {
        using var scope = Services.CreateScope();
        var databaseContext = scope.ServiceProvider.GetRequiredService<AppDataContext>();

        await databaseContext.AddAsync(new ProductDataModel
        {
            Name = "Iphone 15",
            Price = 150000
        }, stoppingToken);

        await databaseContext.SaveChangesAsync(stoppingToken);
    }
}
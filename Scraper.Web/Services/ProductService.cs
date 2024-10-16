namespace Scraper.Web.Services;

public class ProductService : BackgroundService
{
    private IServiceProvider _serviceProvider { get; set; }

    public ProductService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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
        Callback(stoppingToken);
        await Task.CompletedTask;
    }

    private async Task DoProcessingAsync(CancellationToken stoppingToken)
    {
        using var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

        var productWebDriver = serviceScope.ServiceProvider.GetRequiredService<ProductWebDriver>();
        await productWebDriver.ExecuteAsync(stoppingToken);
    }
}
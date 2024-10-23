using Confluent.Kafka;
using Scraper.Archiver.Models.Configs;
using Scraper.Common.Serialization;
using Scraper.Common.Services;
using Scraper.DataAccess;
using Scraper.Domain.Models;

namespace Scraper.Archiver.Services;

public class ProductArchiverService : BackgroundService
{
    private readonly AppConfigModel _appConfig;

    private readonly IServiceProvider _serviceProvider;


    public ProductArchiverService(IAppConfigModel appConfig, IServiceProvider serviceProvider)
    {
        _appConfig = (AppConfigModel)appConfig;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var kafkaTopicBuilderService = _serviceProvider.GetRequiredService<KafkaTopicBuilderService>();
        await kafkaTopicBuilderService.BuildAsync();

        var config = new ConsumerConfig
        {
            BootstrapServers = _appConfig.KafkaConfig.Endpoint,
            GroupId = _appConfig.KafkaConfig.Group,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var consumerBuilder = new ConsumerBuilder<Null, List<ProductDataModel>>(config);
        consumerBuilder.SetValueDeserializer(new ProductListDeserializer());

        using (var consumer = consumerBuilder.Build())
        {
            consumer.Subscribe(_appConfig.KafkaConfig.Topic);

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume(CancellationToken.None);
                    var productList = consumeResult.Message.Value;

                    using var scope = _serviceProvider.CreateScope();
                    var appDataContext = scope.ServiceProvider.GetRequiredService<AppDataContext>();
                    await appDataContext.Products.AddRangeAsync(productList);
                    await appDataContext.SaveChangesAsync();
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error consuming message: {e.Error.Reason}");
            }
        }

        await Task.CompletedTask;
    }
}
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Scraper.Archiver.Services;

public class ProductArchiverService : BackgroundService
{
    private readonly AppConfigModel _appConfig;

    public ProductArchiverService(IOptions<AppConfigModel> options)
    {
        _appConfig = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9093",
            GroupId = _appConfig.KafkaConfig.Group,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
        {
            consumer.Subscribe(_appConfig.KafkaConfig.Topic);

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume(CancellationToken.None);
                    Console.WriteLine($"Received message: {consumeResult.Message.Value} from {consumeResult.TopicPartitionOffset}");
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
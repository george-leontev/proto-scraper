using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Scraper.Common.Models.Configs;

namespace Scraper.Common.Services;

public class KafkaTopicBuilderService
{
    private readonly AppConfigBaseModel _appConfig;

    public KafkaTopicBuilderService(IAppConfigModel appConfig)
    {
        _appConfig = (AppConfigBaseModel)appConfig;
    }

    public async Task BuildAsync()
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _appConfig.KafkaConfig.Endpoint }).Build();
        try
        {
            await adminClient.CreateTopicsAsync(
            [
                new TopicSpecification { Name = _appConfig.KafkaConfig.Topic, ReplicationFactor = 1, NumPartitions = 1 }
            ]);
        }
        catch (CreateTopicsException e)
        {
            if (!e.Message.Contains("already exists"))
            {
                throw new Exception(message: e.Message, innerException: e);
            }
        }
    }
}
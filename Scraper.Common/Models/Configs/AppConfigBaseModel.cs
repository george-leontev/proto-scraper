namespace Scraper.Common.Models.Configs;

public abstract class AppConfigBaseModel : IAppConfigModel
{
    public KafkaConfigModel KafkaConfig { get; set; }
}
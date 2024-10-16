namespace Scraper.Common.Models.Configs;

public class KafkaConfigModel
{
    public required string Endpoint { get; set; }

    public required string Topic { get; set; }

    public required string Group { get; set; }
}
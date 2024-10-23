using Confluent.Kafka;
using HtmlAgilityPack;
using Scraper.Common.Serialization;
using Scraper.Common.Services;
using Scraper.Domain.Models;
using Scraper.Web.Models.Configs;

namespace Scraper.Web.Services;

public class ProductExtractor
{
    private readonly AppConfigModel _appConfig;

    public ProductExtractor(IAppConfigModel appConfig)
    {
        _appConfig = (AppConfigModel)appConfig;
    }

    public async Task ExecuteAsync(string pageSource, CancellationToken stoppingToken)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(pageSource);
        var list = new List<ProductDataModel>();
        var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@data-widget,'searchResults') and contains(@class,'widget-search-result-container')]/child::div/child::div").ToArray();

        foreach (var node in nodes)
        {
            try
            {
                var productHref = node.SelectSingleNode(".//a[contains(@class,'tile-hover-target')]").GetAttributeValue("href", null);
                var productHrefSegments = productHref.Split('?')[0].Split('-');
                var productArticle = productHrefSegments.Last().Replace("/", "");

                var nameNode = node.SelectSingleNode(".//a[contains(@class,'tile-hover-target')]//span[contains(text(), 'iPhone 16')]");
                var priceNode = node.SelectSingleNode(".//span[contains(text(), '₽')]");
                var price = priceNode.InnerText.Replace("₽", "").Replace($"{'\u2009'}", "");

                list.Add(new ProductDataModel
                {
                    Name = nameNode.InnerText.Substring(0, Math.Min(nameNode.InnerText.Length, 255)),
                    Price = int.Parse(price),
                    Uid = int.Parse(productArticle),
                    CheckingDate = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        var config = new ProducerConfig
        {
            BootstrapServers = _appConfig.KafkaConfig.Endpoint,
        };

        var producerBuilder = new ProducerBuilder<Null, List<ProductDataModel>>(config);
        producerBuilder.SetValueSerializer(new ProductListSerializer());

        using var producer = producerBuilder.Build();
        try
        {
            var result = await producer.ProduceAsync(_appConfig.KafkaConfig.Topic, new Message<Null, List<ProductDataModel>> { Value = list });
            Console.WriteLine($"Message was sent to '{result.TopicPartitionOffset}'");
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Error sending message: {e.Error.Reason}");
        }
    }
}
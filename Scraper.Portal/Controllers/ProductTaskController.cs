using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Scraper.Common.Serialization;
using Scraper.DataAccess;
using Scraper.Domain.Models;
using Scraper.Portal.Models;
using Scraper.Portal.Models.Configs;

namespace Scraper.Portal.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ProductTaskController : ControllerBase
{
    private readonly AppDataContext _appDataContext;

    private readonly AppConfigModel _appConfig;


    public ProductTaskController(IOptions<AppConfigModel> options, AppDataContext appDataContext)
    {
        _appConfig = options.Value;
        _appDataContext = appDataContext;
    }

    [HttpPost]
    public async Task<ActionResult<ProductTaskDataModel>> PostTaskAsync(ProductTaskModel productTaskModel)
    {
        var entityEntry = await _appDataContext.ProductTasks.AddAsync(new ProductTaskDataModel
        {
            UserId = 1,
            SearchString = productTaskModel.SearchString
        });
        await _appDataContext.SaveChangesAsync();

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _appConfig.KafkaConfig.Endpoint,
        };

        var producerBuilder = new ProducerBuilder<Null, ProductTaskDataModel>(producerConfig);
        producerBuilder.SetValueSerializer(new ProductTaskSerializer());

        using var producer = producerBuilder.Build();

         try
        {
            var result = await producer.ProduceAsync(_appConfig.KafkaConfig.Topic, new Message<Null, ProductTaskDataModel> { Value = entityEntry.Entity });
        }
        catch (ProduceException<Null, ProductTaskDataModel> e)
        {
            Console.WriteLine($"Error sending message: {e.Error.Reason}");
        }

        return entityEntry.Entity;
    }
}
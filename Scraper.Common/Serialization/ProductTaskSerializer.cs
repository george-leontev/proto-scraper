
using Confluent.Kafka;
using System.Text;
using Scraper.Domain.Models;
using System.Text.Json;

namespace Scraper.Common.Serialization;

public class ProductTaskSerializer : ISerializer<ProductTaskDataModel>
{
    public byte[] Serialize(ProductTaskDataModel data, SerializationContext context)
    {
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, typeof(ProductTaskDataModel)));
    }
}
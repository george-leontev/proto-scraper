
using Confluent.Kafka;
using System.Text;
using Scraper.Domain.Models;
using System.Text.Json;

namespace Scraper.Common.Serialization;

public class ProductListSerializer : ISerializer<List<ProductDataModel>>
{
    public byte[] Serialize(List<ProductDataModel> data, SerializationContext context)
    {
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, typeof(List<ProductDataModel>)));
    }
}
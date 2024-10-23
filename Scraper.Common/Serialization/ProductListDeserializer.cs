using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Scraper.Domain.Models;

namespace Scraper.Common.Serialization;

public class ProductListDeserializer : IDeserializer<List<ProductDataModel>>
{
    public List<ProductDataModel> Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return JsonSerializer.Deserialize<List<ProductDataModel>>(Encoding.UTF8.GetString(data.ToArray()))!;
    }
}
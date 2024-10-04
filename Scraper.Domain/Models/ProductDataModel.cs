using System.ComponentModel.DataAnnotations.Schema;

namespace Scraper.Domain.Models;

[Table("Product", Schema = "Business")]
public class ProductDataModel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int Price { get; set; }
}
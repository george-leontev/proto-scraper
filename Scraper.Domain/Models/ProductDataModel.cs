using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scraper.Domain.Models;

[Table("Product", Schema = "Business")]
public class ProductDataModel
{
    public int Id { get; set; }

    [MaxLength(128)]
    public string? Name { get; set; }

    public int Price { get; set; }
}
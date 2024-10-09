using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scraper.Domain.Models;

[Table("Product", Schema = "Business")]
public class ProductDataModel
{
    public int Id { get; set; }

    [MaxLength(256)]
    public required string Name { get; set; }

    public int Price { get; set; }

    public int Uid { get; set; }

    public DateTime CheckingDate { get; set; }
}
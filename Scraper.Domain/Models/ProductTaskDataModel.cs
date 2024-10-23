using System.ComponentModel.DataAnnotations.Schema;

namespace Scraper.Domain.Models;

[Table("ProductTask", Schema = "Business")]

public class ProductTaskDataModel
{
    public int Id { get; set; }

    public int UserId {get; set;}

    public required string SearchString {get; set;}
}
using HtmlAgilityPack;
using Scraper.DataAccess;
using Scraper.Domain.Models;

namespace Scraper.Web.Services;

public class ProductExtractor
{
    private readonly AppDataContext _appDataContext;

    public ProductExtractor(AppDataContext appDataContext)
    {
        _appDataContext = appDataContext;
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

        await _appDataContext.AddRangeAsync(list, stoppingToken);
        await _appDataContext.SaveChangesAsync(stoppingToken);
    }
}
namespace Scraper.Web.Models.Configs;

public class WebDriverConfigModel
{
    public required string HubUrl { get; set; }

    public required string[] BrowserArgs { get; set; }
}
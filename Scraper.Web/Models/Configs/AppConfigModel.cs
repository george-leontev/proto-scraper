using Scraper.Common.Models.Configs;

namespace Scraper.Web.Models.Configs;

public class AppConfigModel : AppConfigBaseModel
{
    public WebDriverConfigModel? WebDriverConfig { get; set; }
}

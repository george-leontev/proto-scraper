using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Scraper.Domain.Models;
using Scraper.Web.Services;
using SeleniumExtras.WaitHelpers;

public class ProductWebDriver
{
    private readonly ProductExtractor _productExtractor;

    public ProductWebDriver(ProductExtractor productExtractor)
    {
        _productExtractor = productExtractor;
    }

    private void RandomTimeout(int minDuration, int maxDuration)
    {
        var rand = new Random();
        Thread.Sleep(rand.Next(minDuration, maxDuration));
    }

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string hubUrl = "http://localhost:4444/wd/hub";
        var options = new ChromeOptions();
        options.AddArguments([
            "--disable-blink-features=AutomationControlled",
            "--disable-dev-shm-usage",
            "--disable-gpu",
            "--window-size=1024,1080"
        ]);
        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalOption("useAutomationExtension", false);
        options.AddArgument("user-agent=Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.5993.89 Safari/537.36");

        var webDriver = new RemoteWebDriver(new Uri(hubUrl), options.ToCapabilities(), TimeSpan.FromSeconds(180));
        webDriver.Manage().Window.Maximize();
        try
        {
            await webDriver.Navigate().GoToUrlAsync("https://ozon.ru");

            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
            var inputSearchElement = wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@name='text' and @placeholder='Искать на Ozon']")));


            inputSearchElement.SendKeys("iPhone 16");
            RandomTimeout(1000, 2000);
            inputSearchElement.SendKeys(Keys.Enter);

            var endProcessing = false;
            while (!endProcessing)
            {
                IWebElement? nextElement = null;
                var c = 0;
                var list = new List<ProductDataModel>();
                while (nextElement is null && !endProcessing)
                {
                    webDriver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                    RandomTimeout(250, 500);

                    try
                    {
                        nextElement = webDriver.FindElement(By.XPath("//a//div[text()='Дальше']/ancestor::a"));
                    }
                    catch
                    {
                        nextElement = null;
                    }
                    c++;
                    if (c > 5)
                    {
                        endProcessing = true;
                    }
                }

                await _productExtractor.ExecuteAsync(webDriver.PageSource, stoppingToken);

                nextElement?.Click();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            webDriver.Quit();
        }
    }
}
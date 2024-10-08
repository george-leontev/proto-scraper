using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class BackgroundTickService : BackgroundService
{
    public IServiceProvider Services { get; }

    public BackgroundTickService(IServiceProvider services)
    {
        Services = services;
    }

    private void Callback(object? timerState)
    {
        Task.Run(async () =>
        {
            await DoProcessingAsync((CancellationToken)timerState!);
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Callback(stoppingToken);
        await Task.CompletedTask;
    }

    private void RandomTimeout(int minDuration, int maxDuration)
    {
        var rand = new Random();
        Thread.Sleep(rand.Next(3000, 5000));
    }

    private async Task DoProcessingAsync(CancellationToken stoppingToken)
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

            RandomTimeout(1000, 2000);

            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(20));

            IWebElement inputSearchElement = wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@name='text' and @placeholder='Искать на Ozon']")));
            RandomTimeout(1000, 2000);
            inputSearchElement.SendKeys("iphone");
            RandomTimeout(1000, 2000);

            inputSearchElement.SendKeys(Keys.Enter);
            RandomTimeout(1000, 2000);

            IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;

            for (int i = 0; i <= 10; i++)
            {
                IWebElement? nextElement = null;
                while (nextElement is null)
                {
                    js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                    RandomTimeout(250, 500);

                    try
                    {
                        nextElement = webDriver.FindElement(By.XPath("//a//div[text()='Дальше']/ancestor::a"));
                    }
                    catch
                    {
                        nextElement = null;
                    }
                }

                nextElement.Click();
                RandomTimeout(500, 1000);

                Screenshot screenshot = ((ITakesScreenshot)webDriver).GetScreenshot();
                screenshot.SaveAsFile($"/home/george/Pictures/{Guid.NewGuid()}.png");

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

        await Task.CompletedTask;
    }
}
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Scraper.DataAccess;
using Scraper.Domain.Models;
using System.Threading;

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
        // var timer = new Timer(
        //     Callback,
        //     state: stoppingToken,
        //     dueTime: 10000,
        //     period: 60000
        // );
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
            "--window-size=1920,1080"
        ]);
        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalOption("useAutomationExtension", false);
        options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36");


        var webDriver = new RemoteWebDriver(new Uri(hubUrl), options.ToCapabilities(), TimeSpan.FromSeconds(180));
        webDriver.Manage().Window.Maximize();

        await webDriver.Navigate().GoToUrlAsync("https://megamarket.ru");

        // webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);
        RandomTimeout(3000, 5000);

        Screenshot screenshot = ((ITakesScreenshot)webDriver).GetScreenshot();
        screenshot.SaveAsFile($"/home/leo/Pictures/{Guid.NewGuid()}.png");
        webDriver.Quit();


        await Task.CompletedTask;
    }
}
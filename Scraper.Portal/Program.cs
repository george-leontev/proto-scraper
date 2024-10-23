using Microsoft.EntityFrameworkCore;
using Scraper.DataAccess;
using Scraper.Portal.Models.Configs;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Configuration
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

builder.Services.Configure<AppConfigModel>(builder.Configuration);

builder.Services.AddDbContext<AppDataContext>(options =>
{
    var c = builder.Configuration.GetConnectionString("DefaultConnectionString");
});


app.Run();

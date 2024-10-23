using Scraper.DataAccess;
using Microsoft.EntityFrameworkCore;
using Scraper.Web.Services;
using Scraper.Web.Models.Configs;
using Scraper.Common.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();

builder.Configuration
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

var appConfig = new AppConfigModel();
builder.Configuration.Bind(appConfig);
builder.Services.AddSingleton<IAppConfigModel>(appConfig);

builder.Services.AddDbContext<AppDataContext>(options =>
{
    var c = builder.Configuration.GetConnectionString("DefaultConnectionString");
    options.UseSqlServer(c, b => b.MigrationsAssembly("Scraper.Web"));
});

builder.Services.AddTransient<ProductExtractor>();
builder.Services.AddTransient<ProductWebDriver>();

builder.Services.AddTransient<KafkaTopicBuilderService>();

builder.Services.AddHostedService<ProductService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapHealthChecks("/healthz");

app.Run();

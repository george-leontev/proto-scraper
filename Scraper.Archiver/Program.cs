using Microsoft.EntityFrameworkCore;
using Scraper.Archiver.Models.Configs;
using Scraper.Archiver.Services;
using Scraper.Common.Services;
using Scraper.DataAccess;

var builder = WebApplication.CreateBuilder(args);

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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
});

builder.Services.AddHostedService<ProductArchiverService>();
builder.Services.AddTransient<KafkaTopicBuilderService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();

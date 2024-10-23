using Microsoft.EntityFrameworkCore;
using Scraper.Domain.Models;

namespace Scraper.DataAccess;

public class AppDataContext: DbContext
{
    public AppDataContext(DbContextOptions<AppDataContext> options) : base(options)
    {
    }

    public DbSet<ProductDataModel> Products { get; set; }
    public DbSet<ProductTaskDataModel> ProductTasks { get; set; }
}
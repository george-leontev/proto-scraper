using Microsoft.AspNetCore.Mvc;
using Scraper.DataAccess;
using Scraper.Domain.Models;

namespace Scraper.Web.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ProductController : ControllerBase
{
    private readonly AppDataContext _appDataContext;
    private readonly DateTimeService _dateTimeService;

    public ProductController(AppDataContext appDataContext, DateTimeService dateTimeService)
    {
        _appDataContext = appDataContext;
        _dateTimeService = dateTimeService;
    }

    [HttpGet]
    public ActionResult<List<ProductDataModel>> GetProduct()
    {
        var pr = _appDataContext.Products.ToList();
        return pr;
    }

    [HttpGet("utc")]
    public ActionResult<string> GetUtc()
    {
        return _dateTimeService.GetDateTime();
    }
}
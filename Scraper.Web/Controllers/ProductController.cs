using Microsoft.AspNetCore.Mvc;
using Scraper.DataAccess;
using Scraper.Domain.Models;

namespace Scraper.Web.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ProductController : ControllerBase
{
    private readonly AppDataContext _appDataContext;

    public ProductController(AppDataContext appDataContext)
    {
        _appDataContext = appDataContext;
    }

    [HttpGet]
    public ActionResult<List<ProductDataModel>> GetProduct()
    {
        var pr = _appDataContext.Products.ToList();
        return pr;
    }

}
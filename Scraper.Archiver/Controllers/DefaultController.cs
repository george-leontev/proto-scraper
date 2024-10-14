using Microsoft.AspNetCore.Mvc;

namespace Scraper.Web.Controllers;

[Route("/")]
public class DefaultController : ControllerBase
{
    [HttpGet]
    public IActionResult GetDocs()
    {
        return Redirect("/swagger/index.html");
    }
}
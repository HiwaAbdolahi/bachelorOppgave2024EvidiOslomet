using Microsoft.AspNetCore.Mvc;

namespace srcFrontend.Controllers;

public class ExitController : Controller
{
    
    private readonly IConfiguration _configuration;

    public ExitController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public IActionResult Index()
    {
        var apiUrl = _configuration["ApiUrl"];
        ViewBag.ApiUrl = apiUrl;
        return View();
    }
}
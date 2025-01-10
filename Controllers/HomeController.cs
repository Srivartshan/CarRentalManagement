using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CarRentalManagement.Models;

namespace CarRentalManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _logger.LogInformation("HomeController initialized.");
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Index action called.");
        return View();
    }

    public IActionResult Privacy()
    {
        _logger.LogInformation("Privacy action called.");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("Error action called with RequestId: {RequestId}", requestId);
        return View(new ErrorViewModel { RequestId = requestId });
    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UniMarket.Web.Data;
using System.Linq;
using UniMarket.Web.Models;

namespace UniMarket.Web.Controllers;

public class HomeController : Controller
{

    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var products = _context.Products
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToList();

        return View(products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

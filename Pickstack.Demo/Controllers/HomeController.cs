using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pickstack.Demo.Models;

namespace Pickstack.Demo.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index() => View(new ArticleForm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(ArticleForm model)
    {
        ViewData["Posted"] = true;
        return View(model);
    }

    public IActionResult Files() => View();

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
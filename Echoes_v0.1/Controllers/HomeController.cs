using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Echoes_v0._1.Models;
using Echoes_v0._1.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Echoes_v0._1.Controllers;

[Authorize]
public class HomeController : Controller
{
    //private readonly ILogger<HomeController> _logger;
    private static string UserId;
    private static int? PostId;
    private static string UserName;

    public HomeController(IDataAccessLayer indal)
    {
        //dal = indal;
        Globals.dal = indal;
    }

    //public HomeController(ILogger<HomeController> logger)
    //{
    //    _logger = logger;
    //}

    public IActionResult Index()
    {
        return View();
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
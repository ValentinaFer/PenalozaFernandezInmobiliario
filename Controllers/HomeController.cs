using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PenalozaFernandezInmobiliario.Models;

namespace PenalozaFernandezInmobiliario.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ToastMsgViewModel vm = new ToastMsgViewModel();
        vm.ToastMessage = GetToastMessage();
        return View(vm); 
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

    private string GetToastMessage()
    {
        if (TempData.ContainsKey("ToastMessage"))
        {
            var toastMessage = TempData["ToastMessage"] as string;
            TempData.Remove("ToastMessage");
            return toastMessage == null ? "" : toastMessage;
        }
        return "";
    }
}

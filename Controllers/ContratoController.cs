using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PenalozaFernandezInmobiliario.Models;

namespace PenalozaFernandezInmobiliario.Controllers;

public class ContratoController : Controller
{
    private readonly ILogger<InquilinoController> _logger;
    private readonly RepositorioContrato rp;

    public ContratoController(ILogger<InquilinoController> logger)
    {
        _logger = logger;
        rp = new RepositorioContrato();
    }

    public IActionResult Index(List<Contrato> lista, int pageNumber = 1)
    {
        try
        {
            var vm = new IndexContratoViewModel();
            vm.Contratos = new List<Contrato>();
            if (lista == null) //Coming from Home
            {
                
                //lista = rp.GetAllForIndex();
                return View(vm);
            }
            if (lista.Count == 0){
                //lista = rp.GetAllForIndex();
            }
            
            return View(vm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la lista de contratos");
            return RedirectToAction("Error", new { codigo = 500 });
        }
    }

    public IActionResult Error(int codigo){ 
        var res = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
        var RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        var ErrorViewModel = new ErrorViewModel{
            RequestId = RequestId,
        };
        switch (codigo)
        {
            case 404:
                ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
                
                break;
            case 500:
                ViewBag.ErrorMessage = "Sorry, something went wrong on the server";
                break;
            default:
                ViewBag.ErrorMessage = "Sorry, something went wrong";
                break;
        }
        return View(ErrorViewModel);
    }

}

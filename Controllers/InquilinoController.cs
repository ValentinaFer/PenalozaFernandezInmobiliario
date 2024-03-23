using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PenalozaFernandezInmobiliario.Models;

namespace PenalozaFernandezInmobiliario.Controllers;

public class InquilinoController : Controller
{
    private readonly ILogger<InquilinoController> _logger;
    private readonly RepositorioInquilino rp;

    public InquilinoController(ILogger<InquilinoController> logger)
    {
        _logger = logger;
        rp = new RepositorioInquilino();
    }

    public IActionResult Index()
    {
        var lista = rp.GetAll();
        return View(lista);
    }

    public IActionResult Upsert(int id)
    {
        //if el id es 0 quiere decir que se esta creado un Inquilino,
        //Se mandaria un ID especifico mayor a cero si se llegara a estar editando un Inquilino(el inquilido dueño de dicho id)
        if (id > 0){ //se esta editando un inquilino existente
            var inquilino = rp.GetById(id);
            return View(inquilino);
        } else { //se esta creando un nuevo inquilino
            return View();
        }
        
    }
    
}

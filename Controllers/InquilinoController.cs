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
        var lista = rp.GetAllForIndex();
        return View(lista);
    }

    public IActionResult Upsert(int id)
    {
        UpsertInquilinoViewModel viewModel = new();
        //if el id es 0 quiere decir que se esta creado un Inquilino,
        //Se mandaria un ID especifico mayor a cero si se llegara a estar editando un Inquilino(el inquilido dueño de dicho id)
        if (id > 0)
        { //se lleva a view en modo de edicion
            viewModel.Inquilino = rp.GetById(id);
            viewModel.Tittle = "Editando Inquilino";
            return View(viewModel);
        }
        else
        { //se lleva a view en modo de creacion
            viewModel.Tittle = "Creando Inquilino";
            viewModel.Inquilino = new Inquilino();
            return View(viewModel);
        }
    }

    //GIRL(me to me), don't forget to try catch the exception with the rest of the code
    [HttpPost]
    public IActionResult Guardar(UpsertInquilinoViewModel inquilinoViewModel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model state is valid");
                if (inquilinoViewModel.Inquilino.Id > 0)
                {
                    rp.Update(inquilinoViewModel.Inquilino);
                }
                else
                {
                    if (rp.Create(inquilinoViewModel.Inquilino) > 0)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                _logger.LogWarning("Model state is invalid");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar");

        }

        _logger.LogInformation("Received data: {@InquilinoViewModel}", inquilinoViewModel.Inquilino.Nombre);

        return RedirectToAction("Index");

    }

    public IActionResult Eliminar(int id)
    {
        rp.Delete(id);
        return RedirectToAction("Index");
    }

    public IActionResult DetalleInquilino(int id){
        if (id > 0){
            var inq = rp.GetById(id);
            return View(inq);
        } else {
            return RedirectToAction("Index");
        }
    }
}

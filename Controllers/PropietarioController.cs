using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PenalozaFernandezInmobiliario.Models;

namespace PenalozaFernandezInmobiliario.Controllers;

public class PropietarioController : Controller
{
    private readonly ILogger<PropietarioController> _logger;
    private readonly RepositorioPropietario rp;

    public PropietarioController(ILogger<PropietarioController> logger)
    {
        _logger = logger;
        rp = new RepositorioPropietario();
    }

    public IActionResult Index()
    {
        var lista = rp.GetAll();
        return View(lista);
    }

    public IActionResult Upsert(int id)
    {
        UpsertPropietarioViewModel viewModel = new();
        //if el id es 0 quiere decir que se esta creado un Propietario,
        //Se mandaria un ID especifico mayor a cero si se llegara a estar editando un Propietario(el inquilido dueño de dicho id)
        if (id > 0)
        { //se lleva a view en modo de edicion
            viewModel.Propietario = rp.GetById(id);
            viewModel.Tittle = "Editando Propietario";
            return View(viewModel);
        }
        else
        { //se lleva a view en modo de creacion
            viewModel.Tittle = "Creando Propietario";
            viewModel.Propietario = new Propietario();
            return View(viewModel);
        }
    }

    //GIRL(me to me), don't forget to try catch the exception with the rest of the code
    [HttpPost]
    public IActionResult Guardar(UpsertPropietarioViewModel propietarioViewModel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model state is valid");
                if (propietarioViewModel.Propietario.Id > 0)
                {
                    rp.Update(propietarioViewModel.Propietario);
                }
                else
                {
                    if (rp.Create(propietarioViewModel.Propietario) > 0)
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

        _logger.LogInformation("Received data: {@PropietarioViewModel}", propietarioViewModel.Propietario.Nombre);

        return RedirectToAction("Index");

    }


}
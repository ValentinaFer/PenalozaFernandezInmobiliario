using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Roles = "Administrador, Empleado")]
    public IActionResult Index(int pageNumber = 1) //consider receiving list of Inquilinos to show(to show ALL Inquilinos, or filtered ones, for example)
    {

        try
        {
            var lista = rp.GetAllForIndex(10, pageNumber); //Consider allowing user to change page size (from me to me)
            HandleMessagesTableVacia(lista.Count);
            var TotalEntries = rp.getTotalEntries();
            IndexInquilinoViewModel vm = new()
            {

                Inquilinos = lista,
                EsEmpleado = User.IsInRole("Empleado"),
                ToastMessage = GetToastMessage(),
                PageNumber = pageNumber,
                TotalEntries = TotalEntries,
                TotalPages = (int)Math.Ceiling((double)TotalEntries / 10),

            };

            _logger.LogInformation("index method:" + vm.ToastMessage);
            return View(vm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la lista de inquilinos");
            return RedirectToAction("Error", new { codigo = 500 });

        }

    }

    public IActionResult Error(int codigo)
    {
        var res = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
        var RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        var ErrorViewModel = new ErrorViewModel
        {
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

    private void HandleMessagesTableVacia(int listaCount)
    {
        if (listaCount == 0)
        {
            _logger.LogWarning("No hay inquilinos para mostrar");
            ViewData["TablaVacia"] = "No hay inquilinos para mostrar";
        }
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

    [HttpGet("/Inquilino/Editar/{id}")]
    [HttpGet("/Inquilino/Crear")]
    public IActionResult Upsert(int id)
    {
        UpsertInquilinoViewModel viewModel = new();
        //if el id es 0 quiere decir que se esta creado un Inquilino,
        //Se mandaria un ID especifico mayor a cero si se llegara a estar editando un Inquilino(el inquilido dueño de dicho id)
        try
        {
            if (id > 0)
            { //se lleva a view en modo de edicion
                viewModel.Inquilino = rp.GetById(id);
                if (viewModel.Inquilino != null)
                {
                    viewModel.Tittle = "Editando Inquilino n°" + viewModel.Inquilino.Id;
                    return View(viewModel);
                }
                else
                {
                    TempData["Error"] = "No se pudo recuperar el inquilino seleccionado..";
                    return RedirectToAction("Index");
                }

            }
            else
            { //se lleva a view en modo de creacion
                viewModel.Tittle = "Creando Inquilino";
                viewModel.Inquilino = new Inquilino();
                return View(viewModel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al recuperar el inquilino");
            return RedirectToAction("Index");
        }

    }

    //GIRL(me to me), don't forget to try catch the exception on the rest of the code
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
                    return RedirectToAction("Update", inquilinoViewModel.Inquilino);
                }
                else
                {
                    return RedirectToAction("Create", inquilinoViewModel.Inquilino);
                }
            }
            else
            {
                _logger.LogWarning("Model state is invalid");
                TempData["ToastMessage"] = "Se produjo un error al crear o actualizar el inquilino..";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar");

        }

        _logger.LogInformation("Received data for update/create: {@InquilinoViewModel}", inquilinoViewModel.Inquilino.Nombre);

        return RedirectToAction("Index");

    }

    [HttpGet]
    public IActionResult Create(Inquilino inquilino)
    {
        try
        {
            var result = rp.Create(inquilino);
            _logger.LogInformation("Create Result: {result}", result);
            if (result > 0)
            {
                TempData["ToastMessage"] = "Inquilino creado con exito!";
            }
            else
            {
                TempData["ToastMessage"] = "No se pudo crear el inquilino..";
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar"); //remember to be more specific here
            TempData["Error"] = "Se produjo un error al crear el inquilino";
        }
        try
        {
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al redirigir a Index de inquilinos");
            TempData["Error"] = "Se produjo un error al redirigir a Index de inquilinos";
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //check this out later
        }
    }

    [HttpGet]
    public IActionResult Update(Inquilino inquilino)
    {
        try
        {
            var result = rp.Update(inquilino);
            _logger.LogInformation("Update Result: {result}", result);
            if (result > 0)
            {
                TempData["ToastMessage"] = "Inquilino editado con exito!";
            }
            else
            {
                TempData["ToastMessage"] = "No se pudo editar el inquilino..";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar"); //remember to be more specific here
            TempData["Error"] = "Se produjo un error al editar el inquilino";
        }
        try
        {
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al redirigir a Index de inquilinos");
            TempData["Error"] = "Se produjo un error al redirigir a Index de inquilinos";
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //check this out later
        }
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        try
        {
            _logger.LogInformation("Delete id: {id}", id);
            var result = rp.Delete(id);
            _logger.LogInformation("Update Result: {result}", result);
            if (result > 0)
            {
                TempData["ToastMessage"] = "Inquilino eliminado con exito!";
            }
            else
            {
                TempData["ToastMessage"] = "No se pudo eliminar el inquilino..";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar"); //remember to be more specific here
            TempData["Error"] = "Se produjo un error al eliminar el inquilino";
        }
        try
        {
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al redirigir a Index de inquilinos");
            TempData["Error"] = "Se produjo un error al redirigir a Index de inquilinos";
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //check this out later
        }
    }

    //Shows the view with the data of the selected inquilino
    //if there's a flag in the viewdata, it means that it's attempting to delete the Inquilino
    [HttpGet("/Inquilino/Eliminar/{id}/{eliminateFlag}")]
    [HttpGet("/Inquilino/VerMas/{id}")]
    public IActionResult RenderDetalleInquilino(int id, bool eliminateFlag = false)
    {
        try
        {
            if (id > 0)
            {
                ViewData["EliminandoFlag"] = eliminateFlag;
                var inq = rp.GetById(id);
                if (inq == null)
                {
                    _logger.LogError("Se obtuvo un inquilino nulo");
                    TempData["Error"] = "Se produjo un error al obtener el inquilino";
                    return RedirectToAction("Error", new { codigo = 404 });
                }
                return View("DetalleInquilino", inq);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        catch (MySqlException ex)
        {
            TempData["Error"] = "Se produjo un error al obtener el inquilino";
            _logger.LogError(ex, "Error al obtener el inquilino");
            return RedirectToAction("Error", new { codigo = 500 });
        }
        catch (HttpRequestException)
        {
            _logger.LogError("Error durante el request al obtener el inquilino");
            TempData["Error"] = "Se produjo un error al redirigir a Index de inquilinos";
            return RedirectToAction("Error", new { codigo = 500 });
        }
    }
}

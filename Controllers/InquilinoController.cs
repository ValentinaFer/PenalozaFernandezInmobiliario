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

    public IActionResult Index(int pageNumber)
    {
        try
        {
            _logger.LogInformation("GetHasMorePages:" + pageNumber + rp.getHasMorePages(pageNumber, 10));
            pageNumber = HandlePagination(pageNumber);
            var lista = rp.GetAllForIndex(10, pageNumber); //Consider allowing user to change page size (from me to me)
            
            HandleMessagesTableVacia(lista.Count, pageNumber);
            IndexInquilinoViewModel vm = new()
            {
                Inquilinos = lista,
                ToastMessage = GetToastMessage(),
                PageNumber = pageNumber
            };
            
            _logger.LogInformation("index method:"+vm.ToastMessage);	
            return View(vm);
        } catch (Exception ex) //get more specific here (from me to me)
        {
            _logger.LogError(ex, "Error al cargar la lista de inquilinos");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }

    private int HandlePagination(int pageNumber){ //working on this still (from me to me)
        if (pageNumber <= 0)
        {
            pageNumber = 1;
        }
        return pageNumber;
    }

    private void HandleMessagesTableVacia(int listaCount, int pageNumber)
    {
        if (listaCount == 0)
        {
            if (pageNumber != 1)
            {
                _logger.LogWarning("No hay mas inquilinos para mostrar!");
                //disable next page button?
            }
            else
            {
                _logger.LogWarning("No hay inquilinos para mostrar");
                ViewData["TablaVacia"] = "No hay inquilinos para mostrar";
            }
        }
    }

    private string GetToastMessage()
    {
        if (TempData.ContainsKey("ToastMessage"))
        {
            var toastMessage = TempData["ToastMessage"] as string;
            TempData.Remove("ToastMessage");
            return toastMessage;
        }
        return "";
    }

    public IActionResult Upsert(int id)
    {
        UpsertInquilinoViewModel viewModel = new();
        //if el id es 0 quiere decir que se esta creado un Inquilino,
        //Se mandaria un ID especifico mayor a cero si se llegara a estar editando un Inquilino(el inquilido dueño de dicho id)
        try{
            if (id > 0)
                { //se lleva a view en modo de edicion
                    viewModel.Inquilino = rp.GetById(id);
                    viewModel.Tittle = "Editando Inquilino n°" + viewModel.Inquilino.Id;
                    return View(viewModel);
                }
                else
                { //se lleva a view en modo de creacion
                    viewModel.Tittle = "Creando Inquilino";
                    viewModel.Inquilino = new Inquilino();
                    return View(viewModel);
                }
        } catch (Exception ex){
            _logger.LogError(ex, "Error al buscar el inquilino");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                    return RedirectToAction("Update",inquilinoViewModel.Inquilino);
                }
                else
                {
                    return RedirectToAction("Create", inquilinoViewModel.Inquilino);
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

        return RedirectToAction("Index", new { pageNumber = 1 });

    }

    [HttpGet]
    public IActionResult Create(Inquilino inquilino){
        try{
            var result = rp.Create(inquilino);
            _logger.LogInformation("Create Result: {result}", result);
            if (result > 0){
                TempData["ToastMessage"] = "Inquilino creado con exito!";
            } else {
                TempData["ToastMessage"] = "No se pudo crear el inquilino..";
            }
        
        } catch (Exception ex){
            _logger.LogError(ex, "Error al guardar"); //remember to be more specific here
            TempData["Error"] = "Se produjo un error al crear el inquilino";
        }
        try{
        return RedirectToAction("Index", new { pageNumber = 1 });
        } catch (Exception ex){
            _logger.LogError(ex, "Error al redirigir a Index de inquilinos");
            TempData["Error"] = "Se produjo un error al redirigir a Index de inquilinos";
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //check this out later
        }
    }

    [HttpGet]
    public IActionResult Update(Inquilino inquilino){
        try{
            var result = rp.Update(inquilino);
            _logger.LogInformation("Update Result: {result}", result);
            if (result > 0){
                TempData["ToastMessage"] = "Inquilino editado con exito!";
            } else {
                TempData["ToastMessage"] = "No se pudo editar el inquilino..";
            }
        } catch (Exception ex){
            _logger.LogError(ex, "Error al guardar"); //remember to be more specific here
            TempData["Error"] = "Se produjo un error al editar el inquilino";
        }
        try{
        return RedirectToAction("Index", new { pageNumber = 1 });
        } catch (Exception ex){
            _logger.LogError(ex, "Error al redirigir a Index de inquilinos");
            TempData["Error"] = "Se produjo un error al redirigir a Index de inquilinos";
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //check this out later
        }
    }

    [HttpPost]
    public IActionResult Delete(int id){
        try{
            _logger.LogInformation("Delete id: {id}", id);
            var result = rp.Delete(id);
            _logger.LogInformation("Update Result: {result}", result);
            if (result > 0){
                TempData["ToastMessage"] = "Inquilino eliminado con exito!";
            } else {
                TempData["ToastMessage"] = "No se pudo eliminar el inquilino..";
            }
        } catch (Exception ex){
            _logger.LogError(ex, "Error al eliminar"); //remember to be more specific here
            TempData["Error"] = "Se produjo un error al eliminar el inquilino";
        }
        try{
        return RedirectToAction("redirectToIndex");
        } catch (Exception ex){
            _logger.LogError(ex, "Error al redirigir a Index de inquilinos");
            TempData["Error"] = "Se produjo un error al redirigir a Index de inquilinos";
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //check this out later
        }
    }

    //the call from the button to delete
    [HttpGet]
    public IActionResult Eliminar(int id)
    {
        return RedirectToAction("DetalleInquilino", new { id = id , eliminateFlag = true });
    }

    //Shows the view with the data of the selected inquilino
    //if there's a flag in the viewdata, it means that it's a redirect from the button to delete, thus is the flag to show a form to confirm deletion
    [HttpGet]
    public IActionResult DetalleInquilino(int id, bool eliminateFlag = false)
    {
        try{
            if (id > 0)
            {
                if (eliminateFlag){
                    ViewData["EliminandoFlag"] = true;
                } else {
                    ViewData["EliminandoFlag"] = false;
                }
                _logger.LogInformation("Received flag: {flag}", ViewData["EliminandoFlag"]);
                var inq = rp.GetById(id);
                return View(inq);
            }
            else
            {
                return RedirectToAction("Index", new { pageNumber = 1 });
            }
        }catch(Exception ex)
        {
            _logger.LogError(ex, "Error al redirigir a Index de inquilinos");
            TempData["Error"] = "Se produjo un error al redirigir a Index de inquilinos";
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //check this out later

        }
    }

    public IActionResult redirectToIndex()
    {
        return RedirectToAction("Index", new { pageNumber = 1 });
    }
}

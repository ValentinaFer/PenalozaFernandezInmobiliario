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
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }
            var lista = rp.GetAllForIndex(10, pageNumber);
            if (lista.Count == 0)
            {
                //Inquilino inquilino = new(); //remember to handle this!
                //lista.Add(inquilino);
                lista = rp.GetAllForIndex(10, pageNumber-1);
                pageNumber = pageNumber-1;
            }
            IndexInquilinoViewModel vm = new();
            vm.Inquilinos = lista;
            //vm.ToastMessage = "heeey";
            vm.PageNumber = pageNumber;
            if (TempData.ContainsKey("ToastMessage") && TempData["ToastMessage"] != null){
                vm.ToastMessage = TempData["ToastMessage"] as string;
                TempData["ToastMessage"] = null;
            }
            
            _logger.LogInformation("index method:"+vm.ToastMessage);	
            return View(vm);
        } catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la lista de inquilinos");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

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
        if (rp.Create(inquilino) > 0){
            TempData["ToastMessage"] = "Inquilino creado con exito!";
        } else {
            TempData["ToastMessage"] = "No se pudo crear el inquilino..";
        }
        
        } catch (Exception ex){
             _logger.LogError(ex, "Error al guardar"); //remember to be more specific here
        }
        return RedirectToAction("Index", new { pageNumber = 1 });
    }

    [HttpPut]
    public IActionResult Update(Inquilino inquilino){
        rp.Update(inquilino);
        return RedirectToAction("Index", new { pageNumber = 1 });
    }

    [HttpDelete]
    public IActionResult Eliminar(int id)
    {
        rp.Delete(id);
        return RedirectToAction("Index", new { pageNumber = 1 });
    }

    [HttpGet]
    public IActionResult DetalleInquilino(int id)
    {
        if (id > 0)
        {
            var inq = rp.GetById(id);
            return View(inq);
        }
        else
        {
            return RedirectToAction("Index", new { pageNumber = 1 });
        }
    }
}

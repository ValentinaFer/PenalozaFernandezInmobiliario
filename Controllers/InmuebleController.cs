using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PenalozaFernandezInmobiliario.Models;

namespace PenalozaFernandezInmobiliario.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly ILogger<InmuebleController> _logger;
        private readonly RepositorioInmueble rp;

        public InmuebleController(ILogger<InmuebleController> logger)
        {
            _logger = logger;
            rp = new RepositorioInmueble();
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
                    lista = rp.GetAllForIndex(10, pageNumber - 1);
                    pageNumber = pageNumber - 1;
                }
                IndexInmuebleViewModel vm = new()
                {
                    Inmuebles = lista,
                    PageNumber = pageNumber
                };

                if (lista.Count == 0)
                {
                    vm.Error = "No hay Inmuebles disponibles.";
                }

                vm.ToastMessage = "";
                if (TempData.ContainsKey("ToastMessage"))
                {
                    vm.ToastMessage = TempData["ToastMessage"] as string;
                }
                TempData.Remove("ToastMessage");
                _logger.LogInformation("index method:" + vm.ToastMessage);
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de Inmuebles");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }


        public IActionResult Upsert(int id)
        {
            UpsertInmuebleViewModel viewModel = new();
            //if el id es 0 quiere decir que se esta creado un Inmueble,
            //Se mandaria un ID especifico mayor a cero si se llegara a estar editando un Inmueble(el inquilido dueño de dicho id)
            try
            {
                if (id > 0)
                { //se lleva a view en modo de edicion
                    viewModel.Inmueble = rp.GetById(id);
                    viewModel.Tittle = "Editando Inmueble n°" + viewModel.Inmueble.Id;
                    return View(viewModel);
                }
                else
                { //se lleva a view en modo de creacion
                    viewModel.Tittle = "Creando Inmueble";
                    viewModel.Inmueble = new Inmueble();
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar el Inmueble");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

        }

        //GIRL(me to me), don't forget to try catch the exception with the rest of the code
        [HttpPost]
        public IActionResult Guardar(UpsertInmuebleViewModel InmuebleViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Model state is valid");
                    if (InmuebleViewModel.Inmueble.Id > 0)
                    {
                        return RedirectToAction("Update", InmuebleViewModel.Inmueble);
                    }
                    else
                    {
                        return RedirectToAction("Create", InmuebleViewModel.Inmueble);
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

            _logger.LogInformation("Received data: {@InmuebleViewModel}", InmuebleViewModel.Inmueble.Id);

            return RedirectToAction("Index", new { pageNumber = 1 });

        }

        [HttpGet]
        public IActionResult Create(Inmueble Inmueble)
        {
            try
            {
                var result = rp.Create(Inmueble);
                _logger.LogInformation("Create Result: {result}", result);
                if (result > 0)
                {
                    TempData["ToastMessage"] = "Inmueble creado con exito!";
                }
                else
                {
                    TempData["ToastMessage"] = "No se pudo crear el Inmueble..";
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar"); //remember to be more specific here
                TempData["Error"] = "Se produjo un error al crear el Inmueble";
            }
            try
            {
                return RedirectToAction("Index", new { pageNumber = 1 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al redirigir a Index de Inmuebles");
                TempData["Error"] = "Se produjo un error al redirigir a Index de Inmuebles";
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //check this out later
            }
        }

        [HttpGet]
        public IActionResult Update(Inmueble Inmueble)
        {
            try
            {
                var result = rp.Update(Inmueble);
                _logger.LogInformation("Update Result: {result}", result);
                if (result > 0)
                {
                    TempData["ToastMessage"] = "Inmueble editado con exito!";
                }
                else
                {
                    TempData["ToastMessage"] = "No se pudo editar el Inmueble..";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar"); //remember to be more specific here
                TempData["Error"] = "Se produjo un error al editar el Inmueble";
            }
            try
            {
                return RedirectToAction("Index", new { pageNumber = 1 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al redirigir a Index de Inmuebles");
                TempData["Error"] = "Se produjo un error al redirigir a Index de Inmuebles";
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
                    TempData["ToastMessage"] = "Inmueble eliminado con exito!";
                }
                else
                {
                    TempData["ToastMessage"] = "No se pudo eliminar el Inmueble..";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar"); //remember to be more specific here
                TempData["Error"] = "Se produjo un error al eliminar el Inmueble";
            }
            try
            {
                return RedirectToAction("redirectToIndex");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al redirigir a Index de Inmuebles");
                TempData["Error"] = "Se produjo un error al redirigir a Index de Inmuebles";
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //check this out later
            }
        }

        //the call from the button to delete
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            return RedirectToAction("DetalleInmueble", new { id = id, eliminateFlag = true });
        }

        //Shows the view with the data of the selected Inmueble
        //if there's a flag in the viewdata, it means that it's a redirect from the button to delete, thus it the flag to show a form to confirm deletion
        [HttpGet]
        public IActionResult DetalleInmueble(int id, bool eliminateFlag = false)
        {
            try
            {
                if (id > 0)
                {
                    if (eliminateFlag)
                    {
                        ViewData["EliminandoFlag"] = true;
                    }
                    else
                    {
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al redirigir a Index de Inmuebles");
                TempData["Error"] = "Se produjo un error al redirigir a Index de Inmuebles";
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //check this out later

            }
        }

        public IActionResult redirectToIndex()
        {
            return RedirectToAction("Index", new { pageNumber = 1 });
        }
    }
}

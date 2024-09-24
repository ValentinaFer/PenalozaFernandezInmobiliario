using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using PenalozaFernandezInmobiliario.Models;

namespace PenalozaFernandezInmobiliario.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly ILogger<PropietarioController> _logger;
        private readonly RepositorioPropietario rp;

        public PropietarioController(ILogger<PropietarioController> logger)
        {
            _logger = logger;
            rp = new RepositorioPropietario();
        }


        [Authorize(Roles = "Administrador, Empleado")]
        public IActionResult Index(int pageNumber = 1)
        {
            try
            {
                var lista = rp.GetAllForIndex(10, pageNumber);
                HandleMessagesTableVacia(lista.Count);
                var TotalEntries = rp.getTotalEntriesPropietarios();
                IndexPropietarioViewModel vm = new()
                {
                    Propietarios = lista,
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
                _logger.LogError(ex, "Error al cargar la lista de propietarios");
                return RedirectToAction("Error", new { codigo = 500 });
            }
        }


        public IActionResult Upsert(int id)
        {
            UpsertPropietarioViewModel viewModel = new();
            //if el id es 0 quiere decir que se esta creado un Propietario,
            //Se mandaria un ID especifico mayor a cero si se llegara a estar editando un Propietario(el inquilido dueño de dicho id)
            try
            {
                if (id > 0)
                { //se lleva a view en modo de edicion
                    viewModel.Propietario = rp.GetById(id);
                    viewModel.Tittle = "Editando Propietario n°" + viewModel.Propietario.IdPropietario;
                    return View(viewModel);
                }
                else
                { //se lleva a view en modo de creacion
                    viewModel.Tittle = "Creando Propietario";
                    viewModel.Propietario = new Propietario();
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar el Propietario");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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

        private void HandleMessagesTableVacia(int listaCount)
        {
            if (listaCount == 0)
            {
                _logger.LogWarning("No hay propietarios para mostrar");
                ViewData["TablaVacia"] = "No hay propietarios para mostrar";
            }
        }



        [HttpPost]
        public IActionResult Guardar(UpsertPropietarioViewModel PropietarioViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Model state is valid");
                    if (PropietarioViewModel.Propietario.IdPropietario > 0)
                    {
                        return RedirectToAction("Update", PropietarioViewModel.Propietario);
                    }
                    else
                    {
                        return RedirectToAction("Create", PropietarioViewModel.Propietario);
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

            _logger.LogInformation("Received data: {@PropietarioViewModel}", PropietarioViewModel.Propietario.Nombre);

            return RedirectToAction("Index", new { pageNumber = 1 });

        }

        [HttpGet]
        public IActionResult Create(Propietario Propietario)
        {
            try
            {
                var result = rp.Create(Propietario);
                _logger.LogInformation("Create Result: {result}", result);
                if (result > 0)
                {
                    TempData["ToastMessage"] = "Propietario creado con exito!";
                }
                else
                {
                    TempData["ToastMessage"] = "No se pudo crear el Propietario..";
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar"); //remember to be more specific here
                TempData["Error"] = "Se produjo un error al crear el Propietario";
            }
            try
            {
                return RedirectToAction("Index", new { pageNumber = 1 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al redirigir a Index de Propietarios");
                TempData["Error"] = "Se produjo un error al redirigir a Index de Propietarios";
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //check this out later
            }
        }

        [HttpGet]
        public IActionResult Update(Propietario Propietario)
        {
            try
            {
                var result = rp.Update(Propietario);
                _logger.LogInformation("Update Result: {result}", result);
                if (result > 0)
                {
                    TempData["ToastMessage"] = "Propietario editado con exito!";
                }
                else
                {
                    TempData["ToastMessage"] = "No se pudo editar el Propietario..";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar"); //remember to be more specific here
                TempData["Error"] = "Se produjo un error al editar el Propietario";
            }
            try
            {
                return RedirectToAction("Index", new { pageNumber = 1 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al redirigir a Index de Propietarios");
                TempData["Error"] = "Se produjo un error al redirigir a Index de Propietarios";
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //check this out later
            }
        }


        [Authorize(Roles = "Administrador")]
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
                    TempData["ToastMessage"] = "Propietario eliminado con exito!";
                }
                else
                {
                    TempData["ToastMessage"] = "No se pudo eliminar el Propietario..";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar"); //remember to be more specific here
                TempData["Error"] = "Se produjo un error al eliminar el Propietario";
            }
            try
            {
                return RedirectToAction("redirectToIndex");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al redirigir a Index de Propietarios");
                TempData["Error"] = "Se produjo un error al redirigir a Index de Propietarios";
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //check this out later
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            return RedirectToAction("DetallePropietario", new { id = id, eliminateFlag = true });
        }


        [HttpGet]
        public IActionResult DetallePropietario(int id, bool eliminateFlag = false)
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
                _logger.LogError(ex, "Error al redirigir a Index de Propietarios");
                TempData["Error"] = "Se produjo un error al redirigir a Index de Propietarios";
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //check this out later

            }
        }

        public IActionResult redirectToIndex()
        {
            return RedirectToAction("Index", new { pageNumber = 1 });
        }
    }
}

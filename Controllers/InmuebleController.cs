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
        private readonly RepositorioInmueble ri;
        private readonly RepositorioPropietario rp;
        private readonly RepositorioTipoInmueble ti;

        public InmuebleController(ILogger<InmuebleController> logger)
        {
            _logger = logger;
            ri = new RepositorioInmueble();
            rp = new RepositorioPropietario();
            ti = new RepositorioTipoInmueble();
        }

        public IActionResult Index(int pageNumber)
        {
            try
            {
                if (pageNumber <= 0)
                {
                    pageNumber = 1;
                }

                // Obtener la lista de inmuebles
                var listaInmuebles = ri.GetAllForIndex(10, pageNumber);
                if (listaInmuebles.Count == 0)
                {
                    listaInmuebles = ri.GetAllForIndex(10, pageNumber - 1);
                    pageNumber = pageNumber - 1;
                }






                IndexInmuebleViewModel vm = new()
                {
                    Inmuebles = listaInmuebles,
                    PageNumber = pageNumber
                };

                if (listaInmuebles.Count == 0)
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
            UpsertInmuebleViewModel viewModel = new UpsertInmuebleViewModel();
            //if el id es 0 quiere decir que se esta creado un Inmueble,
            //Se mandaria un ID especifico mayor a cero si se llegara a estar editando un Inmueble(el inquilido dueño de dicho id)
            try
            {


                var listaPropietarios = rp.GetAll();
                var listaTipoInmueble = ti.GetTipoInmuebles();

                if (id > 0)
                { //se lleva a view en modo de edicion


                    viewModel.Inmueble = ri.GetById(id);
                    if (viewModel.Inmueble != null)
                    {
                        ViewBag.TipoInmuebles = listaTipoInmueble;
                        ViewBag.Propietarios = listaPropietarios;
                        viewModel.Tittle = "Editando Inquilino n°" + viewModel.Inmueble.IdInmueble;
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
                    viewModel.Tittle = "Creando Inmueble";
                    ViewBag.TipoInmuebles = listaTipoInmueble;
                    ViewBag.Propietarios = listaPropietarios;
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
                    if (InmuebleViewModel.Inmueble.IdInmueble > 0)
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

            _logger.LogInformation("Received data: {@InmuebleViewModel}", InmuebleViewModel.Inmueble.IdInmueble);

            return RedirectToAction("Index", new { pageNumber = 1 });

        }

        [HttpGet]
        public IActionResult Create(Inmueble Inmueble)
        {
            try
            {
                var result = ri.Create(Inmueble);
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
        public IActionResult Update(Inmueble inmueble)
        {
            try
            {
                var result = ri.Update(inmueble);
                _logger.LogInformation("Update Result: {result}", result);
                if (result > 0)
                {
                    TempData["ToastMessage"] = "Inmueble editado con exito!";
                }
                else
                {
                    TempData["ToastMessage"] = "No se pudo editar el inmueble..";
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
                var result = ri.Delete(id);
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


        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            return RedirectToAction("DetalleInmueble", new { id = id, eliminateFlag = true });
        }



        [HttpGet]
        public IActionResult InmuebleDetalles(int id)
        {
            try
            {
                var inmueble = ri.GetById(id);

                if (inmueble == null)
                {
                    _logger.LogWarning("Inmueble con ID {id} no encontrado.", id);
                    return Json(new { success = false, message = "Inmueble no encontrado." });
                }

                var result = new
                {
                    Direccion = inmueble.Direccion,
                    Ambientes = inmueble.Ambientes,
                    Latitud = inmueble.Latitud.ToString("0.000000"),
                    Longitud = inmueble.Longitud.ToString("0.000000"),
                    Superficie = inmueble.Superficie,
                    PropietarioNombre = $"{inmueble.Duenio?.Nombre} {inmueble.Duenio?.Apellido}"
                };

                _logger.LogInformation("Detalles del inmueble con ID {id} obtenidos correctamente.", id);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles del inmueble con ID {id}.", id);
                return Json(new { success = false, message = "Ocurrió un error al obtener los detalles del inmueble." });
            }
        }

        public IActionResult redirectToIndex()
        {
            return RedirectToAction("Index", new { pageNumber = 1 });
        }


        // public IActionResult MostrarMapa(decimal latitud, decimal longitud)
        // {
        //     ViewBag.Latitud = latitud;
        //     ViewBag.Longitud = longitud;
        //     return View();
        // }
    }



}

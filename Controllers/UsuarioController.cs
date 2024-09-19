using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PenalozaFernandezInmobiliario.Models;

namespace PenalozaFernandezInmobiliario.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly RepositorioUsuario ru;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(ILogger<UsuarioController> logger)
        {
            _logger = logger;
            ru = new RepositorioUsuario();

        }



        public IActionResult Index(string nombre, int pageNumber = 1)
        {
            try
            {
                if (pageNumber <= 0)
                {
                    pageNumber = 1;
                }

                // Obtener la lista de todos los usuarios
                var listaUsuarios = ru.GetAllForIndex(10, pageNumber).AsQueryable();

                // Filtrar por nombre o apellido si se proporciona
                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    listaUsuarios = listaUsuarios.Where(u =>
                        (u.Nombre + " " + u.Apellido).Contains(nombre));
                }

                // Convertir a lista y ajustar el paginado
                var usuariosPaginados = listaUsuarios.Skip((pageNumber - 1) * 10).Take(10).ToList();

                // Ajustar el paginado en caso de que no haya usuarios en la página actual
                if (usuariosPaginados.Count == 0 && pageNumber > 1)
                {
                    pageNumber--;
                    usuariosPaginados = listaUsuarios.Skip((pageNumber - 1) * 10).Take(10).ToList();
                }

                IndexUsuarioViewModel vm = new()
                {
                    Usuarios = usuariosPaginados,
                    PageNumber = pageNumber,
                    Nombre = nombre,
                };

                if (usuariosPaginados.Count == 0)
                {
                    vm.Error = "No hay Usuarios disponibles.";
                }

                vm.ToastMessage = TempData.ContainsKey("ToastMessage") ? TempData["ToastMessage"] as string : "";
                TempData.Remove("ToastMessage");

                _logger.LogInformation("index method:" + vm.ToastMessage);
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de Usuarios");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }




        public IActionResult Upsert(int id)
        {
            UpsertUsuarioViewModel viewModel = new UpsertUsuarioViewModel();

            try
            {
                if (id > 0) // Modo de edición
                {
                    viewModel.Usuario = ru.GetById(id);
                    if (viewModel.Usuario != null)
                    {
                        viewModel.Tittle = "Editando Usuario n°" + viewModel.Usuario.IdUsuario;
                        return View(viewModel);
                    }
                    else
                    {
                        TempData["Error"] = "No se pudo recuperar el usuario seleccionado.";
                        return RedirectToAction("Index");
                    }
                }
                else // Modo de creación
                {
                    viewModel.Tittle = "Creando Usuario";
                    viewModel.Usuario = new Usuario();
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar el Usuario");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        public IActionResult Guardar(UpsertUsuarioViewModel usuarioViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // verifica si  el archivo de avatar  se ha cargado
                    if (usuarioViewModel.AvatarFile != null && usuarioViewModel.AvatarFile.Length > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(usuarioViewModel.AvatarFile.FileName);
                        var extension = Path.GetExtension(usuarioViewModel.AvatarFile.FileName);
                        var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/avatars", newFileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            usuarioViewModel.AvatarFile.CopyTo(stream);
                        }

                        usuarioViewModel.Usuario.Avatar = "/avatars/" + newFileName;
                    }


                    if (usuarioViewModel.Usuario.IdUsuario > 0)
                    {
                        ru.Update(usuarioViewModel.Usuario);
                        TempData["ToastMessage"] = "Usuario editado con éxito!";
                    }
                    else
                    {
                        ru.Create(usuarioViewModel.Usuario);
                        TempData["ToastMessage"] = "Usuario creado con éxito!";
                    }

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el usuario");
                ModelState.AddModelError("", "Ocurrió un error al guardar el usuario.");
            }

            // Si algo falla, retorna la vista original con el modelo
            return View("Upsert", usuarioViewModel);
        }

        [HttpPost]
        public IActionResult Create(Usuario usuario)
        {
            try
            {
                var result = ru.Create(usuario);
                if (result > 0)
                {
                    TempData["ToastMessage"] = "Usuario creado con éxito!";
                }
                else
                {
                    TempData["ToastMessage"] = "No se pudo crear el usuario.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario");
                TempData["Error"] = "Se produjo un error al crear el usuario.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(Usuario usuario)
        {
            try
            {
                var result = ru.Update(usuario);
                if (result > 0)
                {
                    TempData["ToastMessage"] = "Usuario editado con éxito!";
                }
                else
                {
                    TempData["ToastMessage"] = "No se pudo editar el usuario.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar el usuario");
                TempData["Error"] = "Se produjo un error al editar el usuario.";
            }

            return RedirectToAction("Index");
        }


    }
}



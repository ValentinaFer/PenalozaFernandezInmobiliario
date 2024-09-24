using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using PenalozaFernandezInmobiliario.Models;

namespace PenalozaFernandezInmobiliario.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly RepositorioUsuario ru;
        private readonly ILogger<UsuarioController> _logger;
        private readonly IConfiguration configuration;

        public UsuarioController(IConfiguration config, ILogger<UsuarioController> logger)
        {
            _logger = logger;
            configuration = config;
            ru = new RepositorioUsuario();
        }


        [Authorize(Roles = "Administrador")]
        public IActionResult Index(int pageNumber = 1, string nombre = "")
        {
            try
            {
                var lista = ru.GetAllForIndex(10, pageNumber, nombre); // Pasa el parámetro del nombre

                var totalEntries = ru.getTotalEntries(nombre); // Modifica este método para que cuente las entradas filtradas por nombre
                IndexUsuarioViewModel vm = new()
                {
                    EsEmpleado = User.IsInRole("Empleado"),
                    Usuarios = lista,
                    PageNumber = pageNumber,
                    TotalEntries = totalEntries,
                    TotalPages = (int)Math.Ceiling((double)totalEntries / 10),
                };

                _logger.LogInformation("index method:" + vm.ToastMessage);
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de usuarios");
                return RedirectToAction("Error", new { codigo = 500 });
            }
        }



        public IActionResult Upsert(int id)
        {
            UpsertUsuarioViewModel viewModel = new UpsertUsuarioViewModel();

            try
            {
                if (id > 0)
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
                else
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
                    if (!string.IsNullOrEmpty(usuarioViewModel.Usuario.Clave))
                    {
                        string salt = configuration["Salt"];

                        string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: usuarioViewModel.Usuario.Clave,
                            salt: System.Text.Encoding.ASCII.GetBytes(salt),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 1000,
                            numBytesRequested: 256 / 8));

                        usuarioViewModel.Usuario.Clave = hashedPassword;
                    }

                    // Si hay un nuevo archivo de avatar, lo subimos y actualizamos la ruta
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

                    // Si no se selecciona un nuevo avatar, se mantiene el valor del avatar actual
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

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation("Delete id: {id}", id);

                var result = ru.Delete(id);

                _logger.LogInformation("Update Result: {result}", result);

                if (result > 0)
                {
                    TempData["ToastMessage"] = "Usuario eliminado con éxito!";
                }
                else
                {
                    TempData["ToastMessage"] = "No se pudo eliminar el usuario.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar eliminar el usuario");
                TempData["Error"] = "Se produjo un error al intentar eliminar el usuario.";
            }

            return RedirectToAction("Index");
        }




        public IActionResult QuitarAvatar()
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);


            if (string.IsNullOrEmpty(claimValue))
            {
                // Manejar el caso donde el Claim es nulo o vacío
                return BadRequest("No se pudo obtener el identificador del usuario.");
            }

            var userId = int.Parse(claimValue);
            var usuario = ru.GetById(userId);

            if (usuario != null)
            {
                usuario.Avatar = string.Empty;
                ru.Update(usuario);
            }

            return RedirectToAction("Index");
        }

    }


}

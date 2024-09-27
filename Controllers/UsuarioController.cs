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

                var lista = ru.GetAllForIndex(10, pageNumber, nombre);

                var totalEntries = ru.getTotalEntries(nombre);
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


        [Authorize]
        public IActionResult Upsert(int id)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var isAdmin = User.IsInRole("Administrador");

            try
            {
                if (id > 0)
                {

                    var usuario = ru.GetById(id);

                    if (usuario != null)
                    {

                        if (userId != usuario.IdUsuario.ToString() && !isAdmin)
                        {
                            return Forbid();
                        }


                        var viewModel = new UpsertUsuarioViewModel
                        {
                            Usuario = usuario,
                            Tittle = "Editando Usuario n°" + usuario.IdUsuario
                        };

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

                    var viewModel = new UpsertUsuarioViewModel
                    {
                        Tittle = "Creando Usuario",
                        Usuario = new Usuario()
                    };

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
                if (usuarioViewModel.Usuario.IdUsuario > 0) // Si es una edición
                {
                    var usuarioActual = ru.GetById(usuarioViewModel.Usuario.IdUsuario);

                    if (usuarioActual == null)
                    {
                        ModelState.AddModelError("", "El usuario no existe.");
                        return View("Upsert", usuarioViewModel);
                    }

                    // Si el campo de contraseña está vacío, mantenemos la contraseña actual
                    if (string.IsNullOrEmpty(usuarioViewModel.Usuario.Clave))
                    {
                        usuarioViewModel.Usuario.Clave = usuarioActual.Clave;
                    }
                    else
                    {
                        // Hashear la nueva contraseña
                        string salt = configuration["Salt"];

                        string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: usuarioViewModel.Usuario.Clave,
                            salt: System.Text.Encoding.ASCII.GetBytes(salt),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 1000,
                            numBytesRequested: 256 / 8));

                        usuarioViewModel.Usuario.Clave = hashedPassword;
                    }
                }
                else // Si es un nuevo usuario
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
                }

                // Manejo del archivo de avatar
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

                return RedirectToAction("Upsert", new { id = usuarioViewModel.Usuario.IdUsuario });
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

                return BadRequest("No se pudo obtener el identificador del usuario.");
            }

            var userId = int.Parse(claimValue);
            var usuario = ru.GetById(userId);

            if (usuario != null)
            {
                usuario.Avatar = string.Empty;
                ru.Update(usuario);
            }

            return RedirectToAction("Upsert", new { id = userId });
        }



        [Authorize]
        public IActionResult CambiarContrasena(int id)
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (claimValue == null || int.Parse(claimValue) != id)
            {
                return Forbid();
            }

            var viewModel = new CambiarContraseñaViewModel { IdUsuario = id };
            return View(viewModel);
        }


        [HttpPost]
        [Authorize]
        public IActionResult CambiarContrasena(CambiarContraseñaViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (claimValue == null || model.IdUsuario != int.Parse(claimValue))
                    {
                        return Forbid();
                    }


                    var usuario = ru.GetById(model.IdUsuario);

                    if (usuario == null)
                    {
                        ModelState.AddModelError("", "Usuario no encontrado.");
                        return View();
                    }

                    // Verificar la contraseña actual
                    string salt = configuration["Salt"];
                    string hashedCurrentPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: model.ContraseñaActual,
                        salt: System.Text.Encoding.ASCII.GetBytes(salt),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    if (hashedCurrentPassword != usuario.Clave)
                    {
                        ModelState.AddModelError("ContraseñaActual", "La contraseña actual es incorrecta.");
                        return View(model);
                    }

                    // Verificar que la nueva contraseña y su confirmación coincidan
                    if (model.NuevaContraseña != model.ConfirmarContraseña)
                    {
                        ModelState.AddModelError("ConfirmarContraseña", "La nueva contraseña y la confirmación no coinciden.");
                        return View(model);
                    }

                    // Generar el hash de la nueva contraseña
                    string hashedNewPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: model.NuevaContraseña,
                        salt: System.Text.Encoding.ASCII.GetBytes(salt),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    // Actualizar la contraseña en la base de datos
                    usuario.Clave = hashedNewPassword;
                    ru.Update(usuario);

                    TempData["ToastMessage"] = "Contraseña cambiada con exito";

                    return View(model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al cambiar la contraseña");
                    ModelState.AddModelError("", "Ocurrió un error al cambiar la contraseña.");
                }
            }

            return View(model);
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

    }
}

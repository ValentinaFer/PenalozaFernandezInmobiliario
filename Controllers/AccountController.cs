using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using PenalozaFernandezInmobiliario.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using System.Security.Claims;

namespace PenalozaFernandezInmobiliario.Controllers
{
    public class AccountController : Controller
    {
        private readonly RepositorioUsuario ru;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration configuration;

        public AccountController(IConfiguration config, ILogger<AccountController> logger)
        {
            _logger = logger;
            configuration = config;
            ru = new RepositorioUsuario();
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Obtener el usuario por su email o nombre de usuario
                var usuario = ru.ObtenerUsuarioPorEmail(model.Email);
                if (usuario != null)
                {
                    // Recuperar el Salt global desde la configuración
                    string salt = configuration["Salt"];

                    // Hashear la contraseña ingresada con el mismo Salt global
                    string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: model.Password,
                        salt: System.Text.Encoding.ASCII.GetBytes(salt), // Salt global
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    // Comparar la contraseña hasheada con la almacenada en la base de datos
                    if (hashedPassword == usuario.Clave)
                    {
                        // Crear claims para autenticación por cookies
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, usuario.Email),
                            new Claim(ClaimTypes.Role, usuario.RolNombre),
                            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));


                        TempData["ToastMessage"] = "Inicio de sesion exitoso";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // Contraseña incorrecta
                        ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                    }
                }
                else
                {
                    // Usuario no encontrado
                    ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                }
            }

            return View(model);
        }

        // Método para cerrar sesión
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}

using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using PenalozaFernandezInmobiliario.Models;


namespace PenalozaFernandezInmobiliario.Controllers;

[Authorize(Roles = "Administrador, Empleado")]
public class PagoController : Controller
{
    private readonly ILogger<PagoController> _logger;
    private readonly RepositorioContrato rp;
    private readonly RepositorioInquilino rpInq;
    private readonly RepositorioInmueble rpInm;
    private readonly RepositorioTipoInmueble rpTipoInm;
    private readonly RepositorioPago rpP;

    public PagoController(ILogger<PagoController> logger)
    {
        _logger = logger;
        rp = new RepositorioContrato();
        rpInq = new RepositorioInquilino();
        rpInm = new RepositorioInmueble();
        rpTipoInm = new RepositorioTipoInmueble();
        rpP = new RepositorioPago();
    }

    [HttpGet("/Contrato/Pagos/{idContrato}")]
    public IActionResult GetPagos([FromRoute] int idContrato)
    {
        try
        {
            if (idContrato <= 0)
            {
                throw new ArgumentException("No se recibio el ID del contrato.");
            }
            var contrato = rpP.ObtenerPagosPorContrato(idContrato);
            if (contrato == null)
            {
                throw new KeyNotFoundException("¡Contrato no encontrado!");
            }
            var datos = rpP.ObtenerCalculos(contrato);
            return Json(new { calculos = datos, contrato = contrato });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(404, new { message = ex.Message, exceptionType = "KeyNotFoundException" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(404, new { message = ex.Message, exceptionType = "ArgumentException" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { message = "Ocurrio un error inesperado.", error = ex.Message, exceptionType = "Exception" });
        }

    }

    [HttpPost("/Contrato/Pagos/Crear")]
    public IActionResult CrearPago(string detalle, int contratoId)
    {
        try
        {
            if (contratoId <= 0)
            {
                throw new ArgumentException("No se recibio el ID del contrato.");
            }
            if (detalle.Trim() == "" || detalle.Length > 100)
            {
                throw new ArgumentException("El detalle del pago no puede estar vacio, o superar los 100 caracteres.");
            }

            var contrato = rpP.ObtenerPagosPorContrato(contratoId);
            if (contrato == null)
            {
                throw new KeyNotFoundException("¡Contrato no encontrado!");
            }

            var nroPago = rpP.Crear(detalle, contrato);
            if (nroPago <= 0)
            {
                throw new Exception("El pago no pudo ser agregado");
            }
            return Json(nroPago);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { message = "Ocurrio un error inesperado.", error = ex.Message, exceptionType = "InvalidOperationException" });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(404, new { message = ex.Message, exceptionType = "KeyNotFoundException" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(404, new { message = ex.Message, exceptionType = "ArgumentException" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { message = "Ocurrio un error inesperado.", error = ex.Message, exceptionType = "Exception" });
        }

    }

    [HttpPost("/Contrato/Pagos/Editar")]
    public IActionResult EditarPago(string detalleNuevo, int idContrato, int idPago)
    {
        try
        {
            if (idContrato <= 0 || detalleNuevo.Trim().Length == 0 || idPago <= 0)
            {
                throw new ArgumentException("No se recibio el ID del contrato o el detalle del pago");
            }
            if (detalleNuevo.TrimEnd().Length > 100)
            {
                throw new ArgumentException("El detalle del pago no puede superar los 100 caracteres");
            }

            var res = rpP.Editar(idContrato, detalleNuevo, idPago);
            if (res <= 0)
            {
                throw new Exception("Ocurrio un error al editar el pago");
            }
            return Json(res);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(404, new { message = ex.Message, exceptionType = "ArgumentException" });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(404, new { message = ex.Message, exceptionType = "KeyNotFoundException" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { message = "Ocurrio un error inesperado.", error = ex.Message, exceptionType = "Exception" });
        }
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost("/Contrato/Pagos/Eliminar")]
    public IActionResult DeletePago(int idPago, int idContrato)
    {
        try
        {
            if (idContrato <= 0 || idPago <= 0)
            {
                throw new ArgumentException("No se recibio el ID del contrato o el ID del pago");
            }
            var contrato = rp.GetById(idContrato);
            if (contrato == null)
            {
                throw new KeyNotFoundException("No se encontro el contrato");
            }
            Console.WriteLine(idContrato + " " + idPago);
            int res = rpP.Eliminar(contrato, idPago);
            if (res < 0)
            {
                throw new Exception("Ocurrio un error al eliminar el pago");
            }

            return Json(res);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(404, new { message = ex.Message, exceptionType = "ArgumentException" });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(404, new { message = ex.Message, exceptionType = "KeyNotFoundException" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { message = "Ocurrio un error inesperado.", error = ex.Message, exceptionType = "Exception" });
        }
    }
}

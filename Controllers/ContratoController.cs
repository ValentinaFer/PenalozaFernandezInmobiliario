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

public class ContratoController : Controller
{
    private readonly ILogger<ContratoController> _logger;
    private readonly RepositorioContrato rp;
    private readonly RepositorioInquilino rpInq;
    private readonly RepositorioPago rpPago;
    private readonly RepositorioInmueble rpInm;

    private readonly RepositorioTipoInmueble rpTipoInm;

    public ContratoController(ILogger<ContratoController> logger)
    {
        _logger = logger;
        rp = new RepositorioContrato();
        rpInq = new RepositorioInquilino();
        rpInm = new RepositorioInmueble();
        rpTipoInm = new RepositorioTipoInmueble();
        rpPago = new RepositorioPago();
    }


    [Authorize(Roles = "Administrador, Empleado")]
    public IActionResult Index(IList<Contrato> lista, int pageNumber = 1)
    {
        try
        {
            var rol = User.IsInRole("Administrador") ? "Administrador" : "Empleado";
            if (lista.Count == 0 || lista == null) //Coming from Home
            {
                lista = rp.GetAllForIndex(10, pageNumber);

            }
            foreach (var item in lista)
            {
                if (item.Inmueble != null)
                {
                    item.GoogleMapLink = $"https://www.google.com/maps/search/?api=1&query={item.Inmueble.Latitud.ToString().Replace(",", ".")},{item.Inmueble.Longitud.ToString().Replace(",", ".")}";
                }
                else
                {
                    item.GoogleMapLink = "";
                }
            }
            IndexContratoViewModel vm = new()
            {
                Rol = rol,
                Contratos = lista,
                ToastMessage = GetToastMessage(),
                PageNumber = pageNumber,
                TotalEntries = rp.GetTotalEntries(),
                TotalPages = (int)Math.Ceiling((double)rp.GetTotalEntries() / 10),
            };

            return View(vm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la lista de contratos");
            return RedirectToAction("Error", new { codigo = 500 });
        }
    }

    private void HandleMessagesTableVacia(int listaCount)
    {
        if (listaCount == 0)
        {
            _logger.LogWarning("No hay inquilinos para mostrar");
            ViewData["TablaVacia"] = "No hay inquilinos para mostrar";
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


    [HttpGet("/Contrato/delete/{id}/{eliminateFlag}")]
    [HttpGet("/Contrato/VerMas/{id}")]
    public IActionResult RenderDetails(int id, bool eliminateFlag = false)
    {
        try
        {
            ViewData["EliminandoFlag"] = eliminateFlag;
            var c = rp.GetById(id);
            if (c == null)
            {
                _logger.LogError("Se obtuvo algun elemento nulo al recuperar el contrato");
                TempData["Error"] = "Se produjo un error al obtener el contrato";
                return RedirectToAction("Index");
            }
            else
            {
                return View("DetalleContrato", c);
            }


        }
        catch (MySqlException ex)
        {
            TempData["Error"] = "Se produjo un error al obtener el contrato";
            _logger.LogError(ex, "Error al obtener el contrato");
            return RedirectToAction("Error", new { codigo = 500 });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error durante el request al obtener el contrato");
            TempData["Error"] = "Se produjo un error al redirigir a Index de contratos";
            return RedirectToAction("Error", new { codigo = 500 });
        }
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        try{
            _logger.LogInformation("Delete id: {id}", id);
            var result = rp.Delete(id);
            _logger.LogInformation("Update Result: {result}", result);
            if (result > 0)
            {
                TempData["ToastMessage"] = "Contrato eliminado con exito!";
            }
            else
            {
                TempData["ToastMessage"] = "No se pudo eliminar el contrato..";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar"); //remember to be more specific here
            TempData["Error"] = "Se produjo un error al eliminar el contrato";
        }
    
        return RedirectToAction("Index");
    }

    [HttpGet("/Contrato/Editar/{id}")]
    [HttpGet("/Contrato/Crear")]
    public IActionResult Upsert(int id)
    { //Renovar / crear

        try
        {
            UpsertContratoViewModel vm = new UpsertContratoViewModel();
            vm.Inmuebles = (List<Inmueble>?) rpInm.GetAll();
            vm.Inquilinos = (List<Inquilino>?)rpInq.GetAll();
            vm.TipoInmuebles = (List<TipoInmueble>?)rpTipoInm.GetTipoInmuebles();

            if (id > 0)
            { //editando
                vm.Contrato = rp.GetById(id);
                if (vm.Contrato == null)
                {
                    TempData["ToastMessage"] = "No se pudo recuperar el contrato seleccionado!";
                    return RedirectToAction("Index");
                }
                vm.Tittle = "Editando Contrato n°" + vm.Contrato.Id;
                return View("Upsert", vm);
            }
            else
            { //creando
                vm.Contrato = new Contrato
                {
                    Inmueble = new Inmueble
                    {
                        Duenio = new Propietario(),
                    },
                    Inquilino = new Inquilino(),
                };
                vm.Tittle = "Creando nuevo contrato";
                return View("Upsert", vm);
            }

        }
        catch (MySqlException ex)
        {
            TempData["Error"] = "Se produjo un error de sql al obtener el contrato";
            _logger.LogError(ex, "Error al obtener el contrato, Upsert, sql error");
            return RedirectToAction("Error", new { codigo = 500 });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error durante el request al obtener el contrato");
            TempData["Error"] = "Se produjo un error al redirigir a Index de contratos";
            return RedirectToAction("Error", new { codigo = 500 });
        }
    }

    public IActionResult Error(int codigo)
    {
        var res = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
        var RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        var ErrorViewModel = new ErrorViewModel
        {
            RequestId = RequestId,
        };
        switch (codigo)
        {
            case 404:
                ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";

                break;
            case 500:
                ViewBag.ErrorMessage = "Sorry, something went wrong on the server";
                break;
            default:
                ViewBag.ErrorMessage = "Sorry, something went wrong";
                break;
        }
        return View(ErrorViewModel);
    }
    
    [HttpPost("/Contrato/Crear")]
    public IActionResult Crear(int inquilinoId, int inmuebleId, string dateFrom, string dateTo)
    {
        try
        {
            if (inquilinoId <= 0 || inmuebleId <= 0){
                throw new ArgumentException("Debe seleccionar un inquilino y un inmueble.");
            }
            if (dateFrom == "" || dateTo == ""){
                throw new ArgumentException("Las fechas no pueden estar vacias.");
            }
            bool isValidFrom = DateTime.TryParseExact(dateFrom, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dFrom);
            bool isValidTo = DateTime.TryParseExact(dateTo, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dTo);

            if (!isValidFrom || !isValidTo){
                throw new ArgumentException("Las fechas no son validas.");
            }
            if (dFrom > dTo){
                throw new ArgumentException("La fecha de inicio no puede ser mayor a la fecha de fin.");
            }

            Console.WriteLine($"Crear inquilinoId: {inquilinoId} inmuebleId: {inmuebleId} fechaDesde: {dateFrom} fechaHasta: {dateTo}");
            
            var inmueble = rpInm.GetAllDetailsById(inmuebleId);
            if (inmueble == null){
                throw new KeyNotFoundException("El inmueble no fue encontrado!");
            }
            if (!rpInm.CheckDisponibilidad(inmuebleId, dFrom, dTo)){
                throw new Exception("Fuera de disponibilidad");
            }
            int id = rp.Create(inquilinoId, inmueble, dFrom, dTo);
            if (id <= 0){
                throw new InvalidOperationException("No se pudo crear el contrato.");
            }

            return Json(id);

        } catch ( KeyNotFoundException ex){
            _logger.LogError(ex, ex.Message);
            return StatusCode(404, new { message = ex.Message, exceptionType = "KeyNotFoundException" });
        } catch (InvalidOperationException ex){ {
            
            _logger.LogError(ex, ex.Message);
            return StatusCode(400, new { message = ex.Message, exceptionType = "InvalidOperationException" });
        }
        } catch (ArgumentException ex){
            
            _logger.LogError(ex, ex.Message);
            return StatusCode(400, new { message = ex.Message, exceptionType = "ArgumentException" });
        } 
        catch (Exception ex)
        {
            if (ex.Message == "Fuera de disponibilidad"){
                return StatusCode(400, new { message = ex.Message, exceptionType = "Fuera de disponibilidad" });
            }
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
        }
        
    }

    [HttpGet] //For DATATABLE
    public IActionResult GetInmuebles([FromQuery] string startDate, [FromQuery] string endDate, [FromQuery] string usoInmueble)
    {
        try
        {
            var vm = new ContratoDataTableViewModel<Inmueble>();

            Console.WriteLine($"startDate: {startDate} endDate: {endDate}");
            DateTime startD = new();
            DateTime endD = new();
            if (startDate != "" && endDate != "" && startDate != null && endDate != null)
            {
                startD = DateTime.ParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture); //this shouldn't be actually working..
                endD = DateTime.ParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                Console.WriteLine($"After format, startDate: {startD} endDate: {endD}");
            }

            vm.Draw = HttpContext.Request.Query["draw"].FirstOrDefault();
            var startRecord = Request.Query["start"].FirstOrDefault();
            var length = Request.Query["length"].FirstOrDefault();
            var sortColumn = Request.Query["columns[" + Request.Query["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Query["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Query["search[value]"].FirstOrDefault();
            searchValue = searchValue == null ? "" : searchValue;

            sortColumn = sortColumn == null ? "" : sortColumn;
            sortColumnDirection = sortColumnDirection == null ? "asc" : sortColumnDirection;

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = startRecord != null ? Convert.ToInt32(startRecord) : 0;
            
            Console.WriteLine($"pageSize: {pageSize} skip: {skip} sortColumn: {sortColumn} sortColumnDirection: {sortColumnDirection} searchValue: {searchValue}");

            vm.Data = rpInm.GetAllInRange(startD, endD, searchValue, sortColumn, sortColumnDirection, skip, pageSize);
            vm.RecordsFiltered = rpInm.GetCountGetAllInRangeFiltered(startD, endD, searchValue);
            vm.RecordsTotal = vm.RecordsFiltered;
            Console.WriteLine($"totalRecordsFiltered: {vm.RecordsFiltered} totalRecords: {vm.RecordsTotal}");
            foreach (var inmueble in vm.Data)
            {
                Console.WriteLine($"inmueble: {inmueble.IdInmueble}, {inmueble.Precio}");
            }
            return Json(vm);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }

    }

    [HttpGet("/Contrato/Inquilinos")] //For DATATABLE
    public IActionResult GetInquilinos(){
        try
        {
            var vm = new ContratoDataTableViewModel<Inquilino>();

            vm.Draw = HttpContext.Request.Query["draw"].FirstOrDefault();
            var startRecord = Request.Query["start"].FirstOrDefault();
            var length = Request.Query["length"].FirstOrDefault();
            var sortColumn = Request.Query["columns[" + Request.Query["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Query["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Query["search[value]"].FirstOrDefault();
            searchValue = searchValue == null ? "" : searchValue;

            sortColumn = sortColumn == null ? "" : sortColumn;
            sortColumnDirection = sortColumnDirection == null ? "asc" : sortColumnDirection;

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = startRecord != null ? Convert.ToInt32(startRecord) : 0;
            Console.WriteLine($"pageSize: {pageSize} skip: {skip} sortColumn: {sortColumn} sortColumnDirection: {sortColumnDirection} searchValue: {searchValue}");

            vm.Data = rpInq.GetAllDataTable(searchValue, sortColumn, sortColumnDirection, skip, pageSize);
            vm.RecordsFiltered = rpInq.GetCountGetAllDataTable(searchValue);
            vm.RecordsTotal = vm.RecordsFiltered;
            Console.WriteLine($"totalRecordsFiltered: {vm.RecordsFiltered} totalRecords: {vm.RecordsTotal}");
 
            return Json(vm);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }

    }

    [HttpGet("/Contrato/Inmueble/GetOccupiedMonths/{idInmueble}")]
    public IActionResult GetOccupiedDates(int idInmueble){
        try
        {
            if (idInmueble <= 0){
                throw new ArgumentException("No se recibio el ID del inmueble.");
            }
            return Json(rp.GetOccupiedDates(idInmueble));
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(404, new { message = ex.Message, exceptionType = "KeyNotFoundException" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { message = "Ocurrio un error inesperado.", error = ex.Message });
        }
    }

    [HttpGet("/Contrato/Data/{idContrato}")]
    public IActionResult GetData([FromRoute]int idContrato){
        try{
            if (idContrato <= 0){
                throw new ArgumentException("No se recibio el ID del contrato.");
            }
            var contrato = rpPago.ObtenerPagosPorContrato(idContrato);
            if (contrato == null){
                throw new KeyNotFoundException("No se encontro el contrato.");
            }
            return Json(contrato);
        } 
        catch(KeyNotFoundException ex){
            _logger.LogError(ex, ex.Message);
            return StatusCode(404, new { message = ex.Message, exceptionType = "KeyNotFoundException" });
        }
        catch(ArgumentException ex){
            _logger.LogError(ex, ex.Message);
            return StatusCode(404, new { message = ex.Message, exceptionType = "KeyNotFoundException" });
        }
        catch(Exception ex){
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { message = "Ocurrio un error inesperado.", error = ex.Message });
        }
    }

    [HttpPost("/Contrato/Cancelar/{idContrato}")]
    public IActionResult CancelarAntes([FromRoute]int idContrato){
        try{
            if (idContrato <= 0){
                throw new ArgumentException("No se recibio el ID del contrato.");
            }
            Console.WriteLine($"Cancelar: {idContrato}");
            var res = rp.CancelarContrato(idContrato);
            if (res < 0){
                throw new Exception("Ocurrio un error al cancelar el contrato.");
            }
            return Json("cancelado");
        } catch (Exception ex) {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { message = "Ocurrio un error inesperado.", error = ex.Message });
        }
        
    }

    public IActionResult getOccupiedDatesDiscrim(int idContrato, int idInmueble){
        try {
            if (idContrato <= 0 || idInmueble <= 0) {
                throw new ArgumentException("No se recibio el ID del contrato o del inmueble.");
            }

            return Json(rp.getOccupiedDatesDiscrim(idContrato, idInmueble));
        } catch (Exception ex) {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { message = "Ocurrio un error inesperado.", error = ex.Message });
        }
    }

}

using PenalozaFernandezInmobiliario.Models;

public class IndexInmuebleViewModel
{
    public IEnumerable<Inmueble>? Inmuebles { get; set; }
    public IEnumerable<Propietario>? Propietarios { get; set; }
    public int PageNumber { get; set; }

    public string? ToastMessage { get; set; }

    public string? Error { get; set; }

    public int TotalPages { get; set; }

    public int TotalEntries { get; set; }
    public string Estado { get; set; }  // Para el filtro de estado
    public int? PropietarioId { get; set; }  // Para el filtro de propietario
}


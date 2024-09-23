using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class IndexContratoViewModel
{
    public IEnumerable<Contrato>? Contratos { get; set; }
    public int PageNumber { get; set; }

    public string? ToastMessage { get; set; }

    public string? Error { get; set; }

    public int TotalPages { get; set; }

    public int TotalEntries { get; set; }
    public bool EsEmpleado { get; set; }

}


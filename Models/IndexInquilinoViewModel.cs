using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class IndexInquilinoViewModel
{
    public IEnumerable<Inquilino>? Inquilinos { get; set; }
    public int PageNumber { get; set; }

    public string? ToastMessage { get; set; }

    public string? Error { get; set; }

    public bool HasMorePages { get; set; }

    public int TotalPages { get; set; }

    public int TotalEntries { get; set; }

}


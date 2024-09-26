using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class ContratoDataTableViewModel<T>
{
    public string? Draw { get; set; }
    public int? RecordsFiltered { get; set; } 
    public int? RecordsTotal { get; set; }
    public List<T>? Data { get; set; }
}

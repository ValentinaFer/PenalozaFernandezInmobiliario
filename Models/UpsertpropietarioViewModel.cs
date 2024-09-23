using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class UpsertPropietarioViewModel
{
    public string? Tittle { get; set; }
    public string EntityName { get; set; } = "propietario";
    public Propietario? Propietario { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class UpsertPropietarioViewModel
{
    public string? Tittle { get; set; }
    public Propietario? Propietario { get; set; }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace PenalozaFernandezInmobiliario.Models;

[Table("Inmuebles")]

public class Inmueble
{


    public int Id { get; set; }
    public string? Direccion { get; set; }
    public int Ambientes { get; set; }
    public int Superficie { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public int PropietarioId { get; set; }
    [ForeignKey(nameof(PropietarioId))]
    public Propietario? Duenio { get; set; }
    public bool Estado { get; set; }


}
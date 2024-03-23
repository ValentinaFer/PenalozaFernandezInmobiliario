using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class Inquilino
{
    [Key]
    [Display(Name = "Código Int.")]
    public int Id { get; set;}
    [Required]
    public string? Nombre{ get; set;}
    [Required]
    public string? Apellido { get; set;}
    [Required]
    public string? Email { get; set;}
    [Required]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set;}
    [Required]
    public string? Dni { get; set;}
    [Required]
    public string? Domicilio { get; set;}

}

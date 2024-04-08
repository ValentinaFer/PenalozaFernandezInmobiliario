using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PenalozaFernandezInmobiliario.Models;

[Table("Inquilinos")]
public class Inquilino
{
    [Key]
    [Display(Name = "Código Int.")]
    public int Id { get; set;}
    [Required]
    [MaxLength(100)]
    [Display(Name = "Nombre")]
    public string? Nombre{ get; set;}
    [Required]
    [MaxLength(100)]
    public string? Apellido { get; set;}
    [Required, EmailAddress]
    [MaxLength(100)]
    public string? Email { get; set;}
    [Required]
    [MaxLength(15)]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set;}
    [Required]
    [MaxLength(20)]
    public string? Dni { get; set;}
    [Required]
    [MaxLength(255)]
    public string? Domicilio { get; set;}
    [Display(Name = "Activo")]
    public bool Estado { get; set;}

}

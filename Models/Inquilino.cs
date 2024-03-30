using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class Inquilino
{
    [Key]
    [Display(Name = "Código Int.")]
    public int Id { get; set;}
    [Required(ErrorMessage ="Por favor, ingrese el {0} del inquilino")]
    public string? Nombre{ get; set;}
    [Required(ErrorMessage ="Por favor, ingrese el {0} del inquilino")]
    public string? Apellido { get; set;}
    [Required(ErrorMessage ="Por favor, ingrese el {0} del inquilino"), EmailAddress(ErrorMessage ="Por favor, ingrese un email válido")]
    public string? Email { get; set;}
    [Required(ErrorMessage ="Por favor, ingrese el {0} del inquilino")]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set;}
    [Required(ErrorMessage ="Por favor, ingrese el {0} del inquilino")]
    public string? Dni { get; set;}
    [Required(ErrorMessage ="Por favor, ingrese el {0} del inquilino")]
    public string? Domicilio { get; set;}
    [Display(Name = "Activo")]
    public bool Estado { get; set;}

}

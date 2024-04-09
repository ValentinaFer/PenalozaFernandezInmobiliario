using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class Propietario
{
    [Key]
    [Display(Name = "Código Int.")]
    public int IdPropietario { get; set; }
    [Required(ErrorMessage = "Por favor, ingrese el {0} del propietario")]
    public string? Nombre { get; set; }
    [Required(ErrorMessage = "Por favor, ingrese el {0} del propietario")]
    public string? Apellido { get; set; }
    [Required(ErrorMessage = "Por favor, ingrese el {0} del propietario"), EmailAddress(ErrorMessage = "Por favor, ingrese un email válido")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Por favor, ingrese el {0} del propietario")]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }
    [Required(ErrorMessage = "Por favor, ingrese el {0} del propietario")]
    public string? Dni { get; set; }
    [Required(ErrorMessage = "Por favor, ingrese el {0} del propietario")]
    public string? Domicilio { get; set; }
    public bool Estado { get; set; }

}

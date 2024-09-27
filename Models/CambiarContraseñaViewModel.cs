using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models
{
    public class CambiarContraseñaViewModel
    {

        public int IdUsuario { get; set; }
        public string? ToastMessage { get; set; }
        public string? Rol { get; set; }
        public string? Error { get; set; }

        [Required(ErrorMessage = "Debe ingresar la contraseña actual.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string ContraseñaActual { get; set; }

        [Required(ErrorMessage = "Debe ingresar la nueva contraseña.")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y un máximo de {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string NuevaContraseña { get; set; }

        [Required(ErrorMessage = "Debe confirmar la nueva contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [Compare("NuevaContraseña", ErrorMessage = "La nueva contraseña y la confirmación no coinciden.")]
        public string ConfirmarContraseña { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PenalozaFernandezInmobiliario.Models
{
    [Table("Inmuebles")]
    public class Inmueble
    {
        public int IdInmueble { get; set; }
        public string? Direccion { get; set; }
        public int Ambientes { get; set; }
        public int Superficie { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public decimal Precio { get; set; }
        public string? Uso { get; set; }

        [DisplayName("Dueño")]
        public int IdPropietario { get; set; }

        [ForeignKey(nameof(IdPropietario))]
        public Propietario? Duenio { get; set; }

        [DisplayName("Tipo de Inmueble")]
        public int IdTipoInmueble { get; set; }

        [ForeignKey(nameof(IdTipoInmueble))]
        public TipoInmueble? Tipo { get; set; }
        public string? Estado { get; set; }
    }
}

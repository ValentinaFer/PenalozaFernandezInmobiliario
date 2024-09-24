using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PenalozaFernandezInmobiliario.Models
{
    [Table("TipoInmueble")]
    public class TipoInmueble
    {


        public int IdTipoInmueble { get; set; }
        public bool Estado { get; set; }
        public string? Tipo { get; set; }
    }
}








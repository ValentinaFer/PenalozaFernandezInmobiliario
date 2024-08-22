using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PenalozaFernandezInmobiliario.Models
{
    [Table("TipoInmueble")]
    public class TipoInmueble
    {


        public int idTipoInmueble { get; set; }
        public string? tipo { get; set; }
    }
}








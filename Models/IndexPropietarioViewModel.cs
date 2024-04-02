using System.Collections.Generic;

namespace PenalozaFernandezInmobiliario.Models
{
    public class IndexPropietarioViewModel
    {
        public IList<Propietario> Propietarios { get; set; }
        public int PageNumber { get; set; }
        public string ToastMessage { get; set; }
        public string? Error { get; set; }
    }
}



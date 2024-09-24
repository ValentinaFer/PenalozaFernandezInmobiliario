using System.Collections.Generic;

namespace PenalozaFernandezInmobiliario.Models
{
    public class IndexPropietarioViewModel
    {
        public IList<Propietario> Propietarios { get; set; }
        public bool EsEmpleado { get; set; }
        public int PageNumber { get; set; }

        public string? ToastMessage { get; set; }

        public string? Error { get; set; }

        public int TotalPages { get; set; }

        public int TotalEntries { get; set; }
    }
}



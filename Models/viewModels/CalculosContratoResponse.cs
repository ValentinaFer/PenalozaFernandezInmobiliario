using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class CalculosContratoResponse
{
    public int MesesTotales { get; set; }
    public decimal MesesHastaFinalizacion { get; set; }
    public int MesesPagados { get; set; }
    public int MesesFaltantesDePago { get; set; }
    public decimal Multa { get; set; }
    public decimal TotalAPagarConMulta { get; set; }
    public decimal TotalAPagarSinMulta { get; set; }
    public bool ContratoPagado { get; set; }

    public bool Cancelado { get; set; }
}

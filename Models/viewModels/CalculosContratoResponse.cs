using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class CalculosContratoResponse
{
    public bool Vencido { get; set; }
    public int MesesPagadosSinMulta { get; set; }
    public int MesesTotales { get; set; }
    public decimal TotalPagado { get; set;}
    public decimal TotalAPagar { get; set; }
    public decimal FaltanteAPagar { get; set; } //Total a pagar final
    public decimal MesesHastaFinalizacion { get; set; }
    public int MesesMulta { get; set; }
    public decimal Multa { get; set; }
    public int MesesSinPagarSinMulta { get; set; }
    public int MesesSinPagarConMulta { get; set; }
    public bool Cancelado { get; set; }

    public bool ContratoPagado { get; set; }

}

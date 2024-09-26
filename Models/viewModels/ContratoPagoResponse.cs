using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class ContratoPagoResponse
{
    public Contrato? Contrato { get; set; }
    public List<Pago>? Pagos { get; set; }
}

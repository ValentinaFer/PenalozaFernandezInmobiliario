using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class UpsertContratoViewModel
{
    public string? Tittle { get; set; }
    public Contrato? Contrato { get; set; }
    public List<Inmueble>? Inmuebles { get; set; }
    public List<Inquilino>? Inquilinos { get; set; }
    public List<TipoInmueble>? TipoInmuebles { get; set; }

    public int Pagos { get; set; }
}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PenalozaFernandezInmobiliario.Models;

[Table("Contratos")]
public class Contrato
{
    [Key]
    [Display(Name = "Código Int.")]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Fecha de inicio")]
    public DateTime FechaDesde { get; set; }

    [Required]
    [Display(Name = "Fecha estipulada de finalización")]
    public DateTime FechaHasta { get; set; }
    [Display(Name = "Fecha de finalización")]
    public DateTime FechaFinalizacion { get; set; }
    public decimal Monto { get; set; } //Monto mensual, no total, el total puede ser calculado en base a la cant de meses


    [Display(Name = "Inquilino")]
    public int InquilinoId { get; set; }

    [ForeignKey(nameof(InquilinoId))]
    public Inquilino? Inquilino { get; set; }



    [Display(Name = "Inmueble")]
    public int InmuebleId { get; set; }

    [ForeignKey(nameof(InmuebleId))]
    public Inmueble? Inmueble { get; set; }


    public bool Estado { get; set; }

    public static string GetTableName()
    {
        Type type = typeof(Contrato);
        if (type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() is TableAttribute tableAttribute)
        {
            return tableAttribute.Name;
        }
        return type.Name;
    }

    public string GetFechaFormateada(DateTime date)
    {
        return date.ToString("dd 'de' MMMM 'de' yyyy", new CultureInfo("es-ES"));
    }

}

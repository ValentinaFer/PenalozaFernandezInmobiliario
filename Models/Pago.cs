using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PenalozaFernandezInmobiliario.Models;

[Table("Pagos")]
public class Pago
{
    [Key]
    [Display(Name = "Código Int.")]
    public int Id { get; set;}
    
    [Required]
    [Display(Name = "Número de pago")]
    public int NroPago { get; set;}

    [Required]
    [Display(Name = "Fecha de pago")]
    public DateTime FechaPago { get; set;}

    [Required]
    [Display(Name = "Detalle del pago")]
    public string? DetallePago { get; set;}

    [Required]
    public decimal Importe{ get; set;} //Monto mensual, no total, el total puede ser calculado en base a la cant de meses


    [Display(Name = "Contrato")]
    public int 	IdContrato { get; set;}

    [ForeignKey(nameof(IdContrato))]

    public bool Estado { get; set;}


    /*public string? CreadoPor { get; set;}
    public DateTime CreadoEn { get; set;}
    public string? ModificadoPor { get; set;}
    public DateTime ModificadoEn { get; set;}*/

//-----------------------------------------------------

    public static string GetTableName()
    {
        Type type = typeof(Contrato);
        if (type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() is TableAttribute tableAttribute)
        {
            return tableAttribute.Name;
        }
        return type.Name;
    }

    public string GetFechaFormateada(DateTime date){
        return date.ToString("dd 'de' MMMM 'de' yyyy", new CultureInfo("es-ES"));
    }

}

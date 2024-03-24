using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class UpsertInquilinoViewModel
{
    public string? Tittle { get; set;}
    public Inquilino? Inquilino { get; set;}
}

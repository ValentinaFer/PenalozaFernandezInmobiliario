using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class UpsertInquilinoViewModel
{
    public string? Tittle { get; set; }
    public string EntityName { get; set; } = "inqulino";
    public Inquilino? Inquilino { get; set; }
}


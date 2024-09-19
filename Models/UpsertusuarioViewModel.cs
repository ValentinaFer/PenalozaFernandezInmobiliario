using System.ComponentModel.DataAnnotations;

namespace PenalozaFernandezInmobiliario.Models;

public class UpsertUsuarioViewModel
{
    public string? Tittle { get; set; }
    public Usuario? Usuario { get; set; }

    [DataType(DataType.Upload)]
    public IFormFile? AvatarFile { get; set; }
}

using PenalozaFernandezInmobiliario.Models;

public class IndexUsuarioViewModel
{
    public List<Usuario> Usuarios { get; set; }
    public int PageNumber { get; set; }

    public string? ToastMessage { get; set; }

    public string? Error { get; set; }

    public int TotalPages { get; set; }
    public string? Rol { get; set; }

    public string? Nombre { get; set; }

}

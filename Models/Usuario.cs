using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PenalozaFernandezInmobiliario.Models;

public enum enRoles
{

    Administrador = 1,
    Empleado = 2,
}

public class Usuario
{
    [Key]
    [Display(Name = "Código")]
    public int IdUsuario { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Required]
    public string Apellido { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required, DataType(DataType.Password)]
    public string Clave { get; set; }
    public string Avatar { get; set; } = "";
    [NotMapped]
    //public IFormFile? AvatarFile { get; set; }

    public int Rol { get; set; }
    [NotMapped]
    public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";

    public override string ToString()
    {
        return $"{Apellido}, {Nombre}";
    }
}


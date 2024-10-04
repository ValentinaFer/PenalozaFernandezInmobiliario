using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using PenalozaFernandezInmobiliario.Models;

namespace PenalozaFernandezInmobiliario.Api;

[Route("api/[controller]")]
[ApiController]
public class PropietariosController : ControllerBase
{
    private readonly MyDbContext _context;

    public PropietariosController(MyDbContext context){
        _context = context;
    }

    //route: api/Propietarios
    [HttpGet]
    public IActionResult Get()
    {
        Console.WriteLine("Get propietarios");
        var propietarios = _context.Propietarios.ToList();
        return Ok(propietarios);
    }

}


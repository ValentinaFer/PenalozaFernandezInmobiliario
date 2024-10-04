using Microsoft.EntityFrameworkCore;
using PenalozaFernandezInmobiliario.Models;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) {

    }


    public DbSet<Propietario> Propietarios { get; set; }


}
using System.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001", "http://*:5000", "https://*:5001");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";  // Ruta personalizada al login
        options.LogoutPath = "/Logout"; // Ruta personalizada para logout
        options.AccessDeniedPath = "/AccessDenied";  // Ruta para acceso denegado
    });
    
    builder.Services.AddDbContext<MyDbContext>( //para EF usando mySql y Pomelo
        options => options.UseMySql(
            builder.Configuration["ConnectionString:DefaultConnection"],
            new MySqlServerVersion(new Version(8, 0, 32))
        )
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Inmueble/Upsert");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors(x => x // Habilitar CORS (ya que vamos a llamar desde un dominio distinto(API))
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
app.UseStaticFiles();

app.UseRouting();

// Habilitar autenticación y autorización
app.UseAuthentication();  // <-- Esta línea habilita la autenticación
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

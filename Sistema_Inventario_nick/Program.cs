using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Sistema_Inventario_nick.DataContext;
using Sistema_Inventario_nick.Services;


var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Obtener la cadena de conexión desde el archivo de configuración
var cnn = configuration.GetConnectionString("cnn");

// Configurar el contexto de la base de datos con Entity Framework Core
builder.Services.AddDbContext<InventariosDbContext>(options => options.UseSqlServer(cnn));

// Agregar la autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Ruta a la página de inicio de sesión
        options.AccessDeniedPath = "/Account/AccessDenied"; // Ruta en caso de acceso denegado
    });

// Agregar servicios al contenedor.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization(); // Añadir autorización
builder.Services.AddSingleton(new FileService(Path.Combine(Directory.GetCurrentDirectory(), "uploads")));
var app = builder.Build();

// Configurar el middleware de la aplicación.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // El valor predeterminado de HSTS es de 30 días. Puedes cambiar esto para escenarios de producción.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware de autenticación y autorización
app.UseAuthentication(); // Añadir autenticación
app.UseAuthorization();  // Añadir autorización

// Configurar los endpoints de la aplicación
app.UseEndpoints(endpoints =>
{
    // Configurar la ruta predeterminada para que inicie en la vista de Login
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");
});

app.Run();

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Sistema_Inventario_nick.DataContext;
using Sistema_Inventario_nick.Services;


var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Obtener la cadena de conexi�n desde el archivo de configuraci�n
var cnn = configuration.GetConnectionString("cnn");

// Configurar el contexto de la base de datos con Entity Framework Core
builder.Services.AddDbContext<InventariosDbContext>(options => options.UseSqlServer(cnn));

// Agregar la autenticaci�n por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Ruta a la p�gina de inicio de sesi�n
        options.AccessDeniedPath = "/Account/AccessDenied"; // Ruta en caso de acceso denegado
    });

// Agregar servicios al contenedor.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization(); // A�adir autorizaci�n
builder.Services.AddSingleton(new FileService(Path.Combine(Directory.GetCurrentDirectory(), "uploads")));
var app = builder.Build();

// Configurar el middleware de la aplicaci�n.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // El valor predeterminado de HSTS es de 30 d�as. Puedes cambiar esto para escenarios de producci�n.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware de autenticaci�n y autorizaci�n
app.UseAuthentication(); // A�adir autenticaci�n
app.UseAuthorization();  // A�adir autorizaci�n

// Configurar los endpoints de la aplicaci�n
app.UseEndpoints(endpoints =>
{
    // Configurar la ruta predeterminada para que inicie en la vista de Login
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");
});

app.Run();

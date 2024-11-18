using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SisInventarios.Model;
using Sistema_Inventario_nick.DataContext;

namespace SisInventarios.Controllers
{
    public class AccountController : Controller
    {
        private readonly InventariosDbContext dbContext;

        public AccountController(InventariosDbContext context)
        {
            dbContext = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            string hashedPassword = HashPassword(password);

            var user = dbContext.users
                .FirstOrDefault(u => u.email == email && u.password == hashedPassword);

            if (user != null)
            {
                // Crear las Claims de autenticación
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.nombre),
                    new Claim(ClaimTypes.Email, user.email)
                };

                // Crear la identidad del usuario
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Autenticación del usuario con las cookies
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                TempData["Message"] = "Login exitoso!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email o contraseña incorrecta.";
            return View();
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(users user)
        {
            if (!ModelState.IsValid) // Verifica que el modelo sea válido
            {
                return View(user);
            }

            // Verificar si el email ya está registrado
            if (dbContext.users.Any(u => u.email == user.email))
            {
                ModelState.AddModelError("email", "Este email ya está registrado.");
                return View(user);
            }

            // Cifrado de la contraseña
            user.password = HashPassword(user.password);

            // Guardar en la base de datos
            dbContext.users.Add(user);
            dbContext.SaveChanges();

            TempData["Message"] = "Registro exitoso!";
            return RedirectToAction("Login");
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Cerrar sesión
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Login");
        }

        // Método para cifrar la contraseña
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        // Página de acceso denegado
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

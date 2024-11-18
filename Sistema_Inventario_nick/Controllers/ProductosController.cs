using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SisInventarios.Model;
using Sistema_Inventario_nick.DataContext;
using System.Linq;
using System.Threading.Tasks;

namespace SisInventarios.Controllers
{
    public class ProductosController : Controller
    {
        private readonly InventariosDbContext _context;

        public ProductosController(InventariosDbContext context)
        {
            _context = context;
        }

        // LISTAR PRODUCTOS
        public async Task<IActionResult> Index()
        {
            return View(await _context.productos.ToListAsync());
        }

        // CREAR PRODUCTO - GET
        public IActionResult Create()
        {
            return View();
        }

        // CREAR PRODUCTO - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("nombre,descripcionPro,valorUnitario,cantidadDispo")] productos producto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(producto);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Producto creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    TempData["Error"] = "Error al guardar el producto. Inténtalo nuevamente.";
                }
            }
            else
            {
                TempData["Error"] = "El modelo no es válido. Verifica los campos.";
            }

            return View(producto);
        }

        // EDITAR PRODUCTO - GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // EDITAR PRODUCTO - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,nombre,descripcionPro,valorUnitario,cantidadDispo")] productos producto)
        {
            if (id != producto.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Producto actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch
                {
                    TempData["Error"] = "Error al actualizar el producto. Inténtalo nuevamente.";
                }
            }
            else
            {
                TempData["Error"] = "El modelo no es válido. Verifica los campos.";
            }

            return View(producto);
        }

        // ELIMINAR PRODUCTO - GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.productos.FirstOrDefaultAsync(m => m.id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // ELIMINAR PRODUCTO - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.productos.FindAsync(id);
            if (producto != null)
            {
                _context.productos.Remove(producto);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Producto eliminado correctamente.";
            }
            else
            {
                TempData["Error"] = "El producto no fue encontrado.";
            }

            return RedirectToAction(nameof(Index));
        }

        // MÉTODO PRIVADO PARA VALIDAR EXISTENCIA
        private bool ProductoExists(int id)
        {
            return _context.productos.Any(e => e.id == id);
        }
    }
}

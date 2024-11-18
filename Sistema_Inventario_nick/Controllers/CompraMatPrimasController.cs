using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SisInventarios.Model;
using Sistema_Inventario_nick.DataContext;

namespace Sistema_Inventario_nick.Controllers
{
    public class CompraMatPrimasController : Controller
    {
        private readonly InventariosDbContext _context;

        public CompraMatPrimasController(InventariosDbContext context)
        {
            _context = context;
        }

        // GET: CompraMatPrimas
        public async Task<IActionResult> Index()
        {
            var compras = _context.CompraMatPrima
                .Include(c => c.MateriaPrima)
                .Include(c => c.Proveedor);
            return View(await compras.ToListAsync());
        }

        // GET: CompraMatPrimas/Create
        public IActionResult Create()
        {
            ViewData["id_proveedor"] = new SelectList(_context.proveedores, "id", "nombre");
            ViewData["id_matPri"] = new SelectList(_context.materias_primas, "id", "nombre");
            return View();
        }

        // POST: CompraMatPrimas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,id_proveedor,id_matPri,valorUnitario,fechaCompra,cantidadCompra")] CompraMatPrima compraMatPrima)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Inicia la transacción
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    // Guarda el registro de la compra
                    _context.Add(compraMatPrima);
                    await _context.SaveChangesAsync();

                    // Actualiza el stock de la materia prima
                    var materiaPrima = await _context.materias_primas.FindAsync(compraMatPrima.id_matPri);
                    if (materiaPrima == null)
                    {
                        throw new Exception("La materia prima especificada no existe.");
                    }

                    materiaPrima.stock = (materiaPrima.stock ?? 0) + (compraMatPrima.cantidadCompra ?? 0);
                    _context.Update(materiaPrima);
                    await _context.SaveChangesAsync();

                    // Confirma la transacción
                    await transaction.CommitAsync();

                    TempData["Success"] = "Compra registrada y stock actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error al registrar la compra: {ex.Message}");
                }
            }

            // En caso de error, recarga las listas desplegables
            ViewData["id_proveedor"] = new SelectList(_context.proveedores, "id", "nombre", compraMatPrima.id_proveedor);
            ViewData["id_matPri"] = new SelectList(_context.materias_primas, "id", "nombre", compraMatPrima.id_matPri);
            return View(compraMatPrima);
        }

        // GET: CompraMatPrimas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compraMatPrima = await _context.CompraMatPrima.FindAsync(id);
            if (compraMatPrima == null)
            {
                return NotFound();
            }

            ViewData["id_proveedor"] = new SelectList(_context.proveedores, "id", "nombre", compraMatPrima.id_proveedor);
            ViewData["id_matPri"] = new SelectList(_context.materias_primas, "id", "nombre", compraMatPrima.id_matPri);
            return View(compraMatPrima);
        }

        // POST: CompraMatPrimas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,id_proveedor,id_matPri,valorUnitario,fechaCompra,cantidadCompra")] CompraMatPrima compraMatPrima)
        {
            if (id != compraMatPrima.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compraMatPrima);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Compra actualizada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error al actualizar la compra: {ex.Message}");
                }
            }

            // Si ocurre un error, recarga las listas desplegables
            ViewData["id_proveedor"] = new SelectList(_context.proveedores, "id", "nombre", compraMatPrima.id_proveedor);
            ViewData["id_matPri"] = new SelectList(_context.materias_primas, "id", "nombre", compraMatPrima.id_matPri);
            return View(compraMatPrima);
        }

        // GET: CompraMatPrimas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compraMatPrima = await _context.CompraMatPrima
                .Include(c => c.MateriaPrima)
                .Include(c => c.Proveedor)
                .FirstOrDefaultAsync(m => m.id == id);

            if (compraMatPrima == null)
            {
                return NotFound();
            }

            return View(compraMatPrima);
        }

        // POST: CompraMatPrimas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var compraMatPrima = await _context.CompraMatPrima.FindAsync(id);
                if (compraMatPrima == null)
                {
                    return NotFound();
                }

                _context.CompraMatPrima.Remove(compraMatPrima);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Compra eliminada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", $"No se puede eliminar la compra: {ex.Message}");
                return RedirectToAction(nameof(Delete), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error inesperado: {ex.Message}");
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}

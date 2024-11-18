using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SisInventarios.Model;
using Sistema_Inventario_nick.DataContext;
using System.Threading.Tasks;

public class ProveedoresController : Controller
{
    private readonly InventariosDbContext _context;
    private readonly IConfiguration _configuration;

    public ProveedoresController(InventariosDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // GET: Proveedores
    public async Task<IActionResult> Index()
    {
        return View(await _context.proveedores.ToListAsync());
    }

    // GET: Proveedores/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Proveedores/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("id,nombre,direccion,tipo_persona,telefono")] proveedores proveedor)
    {
        if (ModelState.IsValid)
        {
            _context.Add(proveedor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(proveedor);
    }

    // GET: Proveedores/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var proveedor = await _context.proveedores.FindAsync(id);
        if (proveedor == null)
        {
            return NotFound();
        }
        return View(proveedor);
    }

    // POST: Proveedores/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("id,nombre,direccion,tipo_persona,telefono")] proveedores proveedor)
    {
        if (id != proveedor.id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(proveedor);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProveedorExists(proveedor.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(proveedor);
    }

    // GET: Proveedores/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var proveedor = await _context.proveedores
            .FirstOrDefaultAsync(m => m.id == id);
        if (proveedor == null)
        {
            return NotFound();
        }

        return View(proveedor);
    }

    // POST: Proveedores/Delete/5
    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var proveedor = await _context.proveedores.FindAsync(id);
        if (proveedor == null)
        {
            return NotFound();
        }

        _context.proveedores.Remove(proveedor);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProveedorExists(int id)
    {
        return _context.proveedores.Any(e => e.id == id);
    }
}

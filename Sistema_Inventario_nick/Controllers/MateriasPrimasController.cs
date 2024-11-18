using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SisInventarios.Model;
using Sistema_Inventario_nick.DataContext;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class MateriasPrimasController : Controller
{
    private readonly InventariosDbContext _context;
    private readonly IConfiguration _configuration;

    public MateriasPrimasController(InventariosDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // Método para obtener las unidades de medida
    private List<string> ObtenerUnidadesDeMedida()
    {
        return new List<string> { "kg", "gr", "lts", "oz", "ml", "lb" };
    }

    // GET: MateriasPrimas
    public async Task<IActionResult> Index()
    {
        return View(await _context.materias_primas.ToListAsync());
    }

    // GET: MateriasPrimas/Create
    public IActionResult Create()
    {
        ViewBag.UnidadesDeMedida = ObtenerUnidadesDeMedida();
        return View();
    }

    // POST: MateriasPrimas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("id,nombre,stock,descripcion,unidad_medida,fecha_vencimiento")] materias_primas materiaPrima)
    {
        if (ModelState.IsValid)
        {
            _context.Add(materiaPrima);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.UnidadesDeMedida = ObtenerUnidadesDeMedida();
        return View(materiaPrima);
    }

    // GET: MateriasPrimas/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var materiaPrima = await _context.materias_primas.FindAsync(id);
        if (materiaPrima == null)
        {
            return NotFound();
        }

        ViewBag.UnidadesDeMedida = ObtenerUnidadesDeMedida();
        return View(materiaPrima);
    }

    // POST: MateriasPrimas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("id,nombre,stock,descripcion,unidad_medida,fecha_vencimiento")] materias_primas materiaPrima)
    {
        if (id != materiaPrima.id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(materiaPrima);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MateriaPrimaExists(materiaPrima.id))
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

        ViewBag.UnidadesDeMedida = ObtenerUnidadesDeMedida();
        return View(materiaPrima);
    }

    // GET: MateriasPrimas/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var materiaPrima = await _context.materias_primas.FirstOrDefaultAsync(m => m.id == id);
        if (materiaPrima == null)
        {
            return NotFound();
        }

        return View(materiaPrima);
    }

    // POST: MateriasPrimas/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var materiaPrima = await _context.materias_primas.FindAsync(id);
        if (materiaPrima == null)
        {
            return NotFound();
        }

        _context.materias_primas.Remove(materiaPrima);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MateriaPrimaExists(int id)
    {
        return _context.materias_primas.Any(e => e.id == id);
    }
}

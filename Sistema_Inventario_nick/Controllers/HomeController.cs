using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sistema_Inventario_nick.DataContext;

namespace Sistema_Inventario_nick.Controllers
{
    public class HomeController : Controller
    {
        private readonly InventariosDbContext _context;

        public HomeController(InventariosDbContext context)
        {
            _context = context;
        }

        // Vista principal para informes
        public IActionResult Index()
        {
            ViewData["Proveedores"] = new SelectList(_context.proveedores, "id", "nombre");
            ViewData["Clientes"] = new SelectList(_context.Clientes, "id", "nombre");
            return View();
        }

        // Generar informe basado en parámetros
        [HttpPost]
        public IActionResult GenerarInforme(string tipo, DateTime? fechaInicio, DateTime? fechaFin)
        {
            if (!fechaInicio.HasValue || !fechaFin.HasValue)
            {
                return BadRequest("Debe proporcionar un rango de fechas válido.");
            }

            var datos = new List<dynamic>();

            if (tipo == "Compras")
            {
                datos = _context.CompraMatPrima
                    .Where(c => c.fechaCompra.HasValue && c.fechaCompra.Value >= fechaInicio.Value && c.fechaCompra.Value <= fechaFin.Value)
                    .Select(c => new
                    {
                        c.id,
                        FechaCompra = c.fechaCompra.Value,
                        Proveedor = c.Proveedor != null ? c.Proveedor.nombre : "Proveedor desconocido",
                        Cantidad = c.cantidadCompra ?? 0,
                        Total = (c.cantidadCompra ?? 0) * (c.valorUnitario ?? 0)
                    })
                    .ToList<dynamic>();
            }
            else if (tipo == "Ventas")
            {
                // Traer ventas a memoria
                var ventas = _context.venta
                    .Where(v => v.FechaVenta >= fechaInicio.Value && v.FechaVenta <= fechaFin.Value)
                    .ToList();

                // Resolver datos en memoria
                var pedidos = _context.pedido.ToList(); // Traer todos los pedidos a memoria
                var clientes = _context.Clientes.ToList(); // Traer todos los clientes a memoria

                datos = ventas.Select(v => new
                {
                    v.Id,
                    FechaVenta = v.FechaVenta,
                    Cliente = pedidos
                        .FirstOrDefault(p => p.id_venta == v.Id)?.id_cliente is int idCliente
                        ? clientes.FirstOrDefault(c => c.id == idCliente)?.nombre ?? "Cliente desconocido"
                        : "Cliente desconocido",
                    TotalVenta = v.TotalVenta
                })
                .ToList<dynamic>();
            }

            return Json(datos);
        }

        // Generar datos para gráficos
        [HttpPost]
        public IActionResult GenerarGrafico(string tipo, DateTime? fechaInicio, DateTime? fechaFin)
        {
            if (!fechaInicio.HasValue || !fechaFin.HasValue)
            {
                return BadRequest("Debe proporcionar un rango de fechas válido.");
            }

            var datos = new List<dynamic>();

            if (tipo == "Compras")
            {
                var query = from c in _context.CompraMatPrima
                            where c.fechaCompra.HasValue && c.fechaCompra.Value >= fechaInicio.Value && c.fechaCompra.Value <= fechaFin.Value
                            group c by c.fechaCompra.Value.Date into g
                            select new
                            {
                                Fecha = g.Key,
                                Total = g.Sum(c => (c.cantidadCompra ?? 0) * (c.valorUnitario ?? 0))
                            };

                datos = query.ToList<dynamic>();
            }
            else if (tipo == "Ventas")
            {
                var query = from v in _context.venta
                            where v.FechaVenta >= fechaInicio.Value && v.FechaVenta <= fechaFin.Value
                            group v by v.FechaVenta.Date into g
                            select new
                            {
                                Fecha = g.Key,
                                Total = g.Sum(v => v.TotalVenta)
                            };

                datos = query.ToList<dynamic>();
            }

            return Json(datos);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SisInventarios.Model;
using Sistema_Inventario_nick.DataContext;
using System.Linq;

namespace SisInventarios.Controllers
{
    public class VentasController : Controller
    {
        private readonly InventariosDbContext _context;

        public VentasController(InventariosDbContext context)
        {
            _context = context;
        }

        // Vista principal de ventas y pedidos
        public IActionResult Index()
        {
            var ventas = _context.venta.ToList();
            var pedidos = _context.pedido.ToList();

            var listado = from v in ventas
                          join p in pedidos on v.Id equals p.id_venta
                          select new
                          {
                              Venta = v,
                              Pedido = p,
                              Cliente = _context.Clientes.FirstOrDefault(c => c.id == p.id_cliente)?.nombre
                          };

            ViewBag.VentasYPedidos = listado;
            return View();
        }

        // Vista para registrar una nueva venta
        public IActionResult NuevaVenta()
        {
            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.Productos = _context.productos.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult RegistrarVenta(int clienteId, List<int> productoIds, List<int> cantidades)
        {
            if (productoIds == null || cantidades == null || productoIds.Count != cantidades.Count)
            {
                return BadRequest("Datos de la venta no válidos.");
            }

            var nuevaVenta = new venta
            {
                FechaVenta = DateTime.Now,
                TotalVenta = 0
            };
            _context.venta.Add(nuevaVenta);
            _context.SaveChanges();

            var nuevoPedido = new pedido
            {
                id_cliente = clienteId,
                id_venta = nuevaVenta.Id
            };
            _context.pedido.Add(nuevoPedido);
            _context.SaveChanges();

            decimal totalVenta = 0;

            for (int i = 0; i < productoIds.Count; i++)
            {
                int productoId = productoIds[i];
                int cantidad = cantidades[i];

                var producto = _context.productos.FirstOrDefault(p => p.id == productoId);

                if (producto == null || producto.cantidadDispo < cantidad)
                {
                    return BadRequest($"El producto {producto?.nombre ?? "desconocido"} no tiene suficiente stock.");
                }

                var detalleVenta = new VenderProd
                {
                    id_pedido = nuevoPedido.id_pedido,
                    id_proc = producto.id,
                    cantidad = cantidad,
                    precioTotalEmpanda = (producto.valorUnitario ?? 0) * cantidad

                };
                _context.venderProd.Add(detalleVenta);

                producto.cantidadDispo -= cantidad;
                _context.productos.Update(producto);

                totalVenta += detalleVenta.precioTotalEmpanda;
            }

            nuevaVenta.TotalVenta = totalVenta;
            _context.venta.Update(nuevaVenta);
            _context.SaveChanges();

            return RedirectToAction("Confirmacion", new { id = nuevaVenta.Id });
        }

        // Vista de confirmación de venta
        public IActionResult Confirmacion(int id)
        {
            var venta = _context.venta.FirstOrDefault(v => v.Id == id);
            var pedido = _context.pedido.FirstOrDefault(p => p.id_venta == id);

            if (venta == null || pedido == null)
            {
                return NotFound("Venta no encontrada.");
            }

            var cliente = _context.Clientes.FirstOrDefault(c => c.id == pedido.id_cliente);
            var detalles = _context.venderProd
                .Where(v => v.id_pedido == pedido.id_pedido)
                .Join(_context.productos,
                      vp => vp.id_proc,
                      p => p.id,
                      (vp, p) => new { p.nombre, vp.cantidad, vp.precioTotalEmpanda })
                .ToList();

            ViewBag.Venta = venta;
            ViewBag.Cliente = cliente;
            ViewBag.Detalles = detalles;

            return View();
        }

        // Vista para editar una venta
        public IActionResult Edit(int id)
        {
            var venta = _context.venta.FirstOrDefault(v => v.Id == id);
            if (venta == null) return NotFound();

            var pedido = _context.pedido.FirstOrDefault(p => p.id_venta == id);
            ViewBag.Pedido = pedido;
            ViewBag.Cliente = _context.Clientes.FirstOrDefault(c => c.id == pedido.id_cliente);
            ViewBag.Detalles = _context.venderProd.Where(vp => vp.id_pedido == pedido.id_pedido)
                                                  .Join(_context.productos,
                                                        vp => vp.id_proc,
                                                        p => p.id,
                                                        (vp, p) => new { p.nombre, vp.cantidad, vp.precioTotalEmpanda })
                                                  .ToList();
            return View(venta);
        }

        [HttpPost]
        public IActionResult Edit(venta ventaEditada)
        {
            var venta = _context.venta.Find(ventaEditada.Id);
            if (venta == null) return NotFound();

            venta.FechaVenta = ventaEditada.FechaVenta;
            venta.TotalVenta = ventaEditada.TotalVenta;

            _context.venta.Update(venta);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Vista para eliminar una venta
        public IActionResult Delete(int id)
        {
            var venta = _context.venta.FirstOrDefault(v => v.Id == id);
            if (venta == null) return NotFound();

            var pedido = _context.pedido.FirstOrDefault(p => p.id_venta == id);

            _context.venderProd.RemoveRange(_context.venderProd.Where(vp => vp.id_pedido == pedido.id_pedido));
            _context.pedido.Remove(pedido);
            _context.venta.Remove(venta);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Detalles de una venta
        public IActionResult Details(int id)
        {
            var venta = _context.venta.FirstOrDefault(v => v.Id == id);
            var pedido = _context.pedido.FirstOrDefault(p => p.id_venta == id);

            if (venta == null || pedido == null) return NotFound();

            var cliente = _context.Clientes.FirstOrDefault(c => c.id == pedido.id_cliente);
            var detalles = _context.venderProd
                .Where(vp => vp.id_pedido == pedido.id_pedido)
                .Join(_context.productos,
                      vp => vp.id_proc,
                      p => p.id,
                      (vp, p) => new { p.nombre, vp.cantidad, vp.precioTotalEmpanda })
                .ToList();

            ViewBag.Venta = venta;
            ViewBag.Cliente = cliente;
            ViewBag.Detalles = detalles;

            return View();
        }
    }
}

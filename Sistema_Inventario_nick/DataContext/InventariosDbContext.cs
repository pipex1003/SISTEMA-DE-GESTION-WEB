using Microsoft.EntityFrameworkCore;
using SisInventarios.Model;

namespace Sistema_Inventario_nick.DataContext
{
    public class InventariosDbContext : DbContext
    {
        public InventariosDbContext(DbContextOptions<InventariosDbContext> options) : base(options) { }

        public DbSet<clientes> Clientes { get; set; }
        public DbSet<materias_primas> materias_primas { get; set; }
        public DbSet<productos> productos { get; set; }
        public DbSet<proveedores> proveedores { get; set; }
        public DbSet<users> users { get; set; }
        public DbSet<CompraMatPrima> CompraMatPrima { get; set; }

        public DbSet<venta> venta { get; set; }
        public DbSet<pedido> pedido { get; set; }
        public DbSet<VenderProd> venderProd { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración para la relación CompraMatPrima con proveedores
            modelBuilder.Entity<CompraMatPrima>()
                .HasOne(c => c.Proveedor)
                .WithMany(p => p.Compras)
                .HasForeignKey(c => c.id_proveedor)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración para la relación CompraMatPrima con materias_primas
            modelBuilder.Entity<CompraMatPrima>()
                .HasOne(c => c.MateriaPrima)
                .WithMany(mp => mp.Compras)
                .HasForeignKey(c => c.id_matPri)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<proveedores>()
                .Property(p => p.id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<materias_primas>()
                .Property(mp => mp.id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<CompraMatPrima>()
                .Property(c => c.id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<VenderProd>()
            .HasKey(vp => new
            {
                vp.id_pedido,
                vp.id_proc
            });

            modelBuilder.Entity<VenderProd>()
                .HasOne(vp => vp.pedido)
                .WithMany(p => p.VenderProds)
                .HasForeignKey(vp => vp.id_pedido);

            modelBuilder.Entity<VenderProd>()
                .HasOne(vp => vp.Producto)
                .WithMany(p => p.VenderProds)
                .HasForeignKey(vp => vp.id_proc);

            // modelBuilder.Entity<venta>().ToTable("venta");
            modelBuilder.Entity<productos>(entity =>
            {
                entity.HasKey(e => e.id);

                entity.Property(e => e.nombre)
                      .HasMaxLength(255)
                      .IsRequired(false); // Campo opcional.

                entity.Property(e => e.descripcionPro)
                      .HasMaxLength(500)
                      .IsRequired(false); // Campo opcional.

                entity.Property(e => e.valorUnitario)
                      .HasColumnType("decimal(10, 2)")
                      .IsRequired(false); // Campo opcional.

                entity.Property(e => e.cantidadDispo)
                      .IsRequired(false); // Campo opcional.
            });
        }
    }
}

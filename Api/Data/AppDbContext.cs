using Microsoft.EntityFrameworkCore;
using Api.Models.Entities;

namespace Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Producto> Productos { get; set; }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Proveedor> Proveedores { get; set; }

        public DbSet<Venta> Ventas { get; set; }

        public DbSet<Compra> Compras { get; set; }

        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        public DbSet<DetalleCompra> DetallesCompra { get; set; }

        public DbSet<Caja> Cajas { get; set; }

        public DbSet<MovimientoCaja> MovimientosCaja { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Example of additional global configurations (optional):
            // modelBuilder.Entity<Usuario>().ToTable("Usuarios");
        }
    }
}
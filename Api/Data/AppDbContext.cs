using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Api.Models.Entities;

namespace Api.Data
{
    public class AppDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

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

            // Identity table names
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UsuarioRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UsuarioClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UsuarioLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UsuarioTokens");
        }
    }
}
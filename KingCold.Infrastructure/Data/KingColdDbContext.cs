using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using KingCold.Domain.Model;

namespace KingCold.Infrastructure.Data
{
    public class KingColdDbContext : DbContext
    {
        public KingColdDbContext(DbContextOptions<KingColdDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Producto { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }
        public DbSet<Servicio> Servicio { get; set; }
        public DbSet<Empleado> Empleado { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<MovimientoInventario> MovimientoInventario { get; set; }
        public DbSet<Categoria> Categoria{ get; set; }
        public DbSet<Venta> Venta { get; set; }
        public DbSet<DetalleVenta> DetalleVenta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Venta>(entity =>
            {
                entity.Property(venta => venta.Total).HasPrecision(10, 2);
                entity.HasOne(venta => venta.Cliente)
                    .WithMany()
                    .HasForeignKey(venta => venta.ClienteId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(venta => venta.Detalles)
                    .WithOne(detalle => detalle.Venta)
                    .HasForeignKey(detalle => detalle.VentaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.Property(detalle => detalle.TipoItem).HasMaxLength(20).IsUnicode(false);
                entity.Property(detalle => detalle.PrecioUnitario).HasPrecision(10, 2);
                entity.Property(detalle => detalle.Subtotal).HasPrecision(10, 2);
            });

            modelBuilder.Entity<Servicio>(entity =>
            {
                entity.Property(servicio => servicio.PrecioBase).HasPrecision(10, 2);
            });
        }
    }
}

using CuponesApiTp.Models;
using Microsoft.EntityFrameworkCore;

namespace CuponesApiTp.Data
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }

        public DbSet<CuponModel> Cupones { get; set; }
        public DbSet<CategoriaModel> Categorias { get; set; }
        public DbSet<Cupon_CategoriaModel> Cupones_Categorias { get; set; }
        public DbSet<Tipo_CuponModel> Tipo_Cupon { get; set; }
        public DbSet<Cupon_ClienteModel> Cupones_Clientes { get; set; }
        public DbSet<Cupones_DetalleModel> Cupones_Detalle { get; set; }
        public DbSet<ArticuloModel> Articulos { get; set; }
        public DbSet<PrecioModel> Precios { get; set; }
        public DbSet<Cupones_HistorialModel> Cupones_Historial { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CuponModel>()
                .HasKey(c => c.Id_Cupon);
            modelBuilder.Entity<CategoriaModel>()
                .HasKey(c => c.Id_Categoria);
            modelBuilder.Entity<Cupon_CategoriaModel>()
                .HasKey(c => c.Id_Cupones_Categorias);
            modelBuilder.Entity<Tipo_CuponModel>()
               .HasKey(c => c.Id_Tipo_Cupon);
            modelBuilder.Entity<Cupon_ClienteModel>()
               .HasKey(c => c.NroCupon);
            modelBuilder.Entity<Cupones_DetalleModel>()
               .HasKey(c => new { c.Id_Cupon, c.Id_Articulo });
            modelBuilder.Entity<ArticuloModel>()
               .HasKey(c => c.Id_Articulo);
            modelBuilder.Entity<PrecioModel>()
               .HasKey(c => c.Id_Precio);
            modelBuilder.Entity<Cupones_HistorialModel>()
               .HasKey(c => new { c.Id_Cupon, c.NroCupon });

            modelBuilder.Entity<ArticuloModel>()
                .HasOne(a => a.Categoria) // Cada artículo tiene una categoría
                .WithMany(c => c.Articulos) // Cada categoría puede tener muchos artículos
                .HasForeignKey(a => a.id_categoria) // Relación por la clave foránea 'id_categoria'
                .OnDelete(DeleteBehavior.SetNull); // Al eliminar la categoría, se establece la categoría a NULL en los artículos (asegúrate de permitir valores NULL en la base de datos)

            // Configuración de relaciones entre Cupones y Categorias
            modelBuilder.Entity<Cupon_CategoriaModel>()
                .HasOne(cc => cc.Cupon)
                .WithMany(c => c.Cupones_Categorias)
                .HasForeignKey(cc => cc.Id_Cupon)
                .OnDelete(DeleteBehavior.Cascade); // Cascada para eliminar los cupones relacionados

            modelBuilder.Entity<Cupon_CategoriaModel>()
                .HasOne(cc => cc.Categoria)
                .WithMany(c => c.Cupones_Categorias)
                .HasForeignKey(cc => cc.Id_Categoria)
                .OnDelete(DeleteBehavior.Cascade); // Cascada para eliminar las relaciones cuando se elimina una categoría

            modelBuilder.Entity<Cupon_ClienteModel>()
                .HasOne(cc => cc.Cupon) // Definir relación entre Cupon_ClienteModel y CuponModel
                .WithMany(c => c.Cupones_Clientes) // Un cupón puede tener muchos registros en Cupon_Clientes
                .HasForeignKey(cc => cc.Id_Cupon) // La clave foránea es 'Id_Cupon'
                .OnDelete(DeleteBehavior.Cascade); // Puedes establecer el comportamiento de eliminación (en este caso, cascada)

            base.OnModelCreating(modelBuilder);
        }
    }
}

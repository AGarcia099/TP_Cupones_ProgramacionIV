using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CuponesApiTp.Models
{
    [Table("Articulos")]
    public class ArticuloModel
    {
        [Key]
        public int Id_Articulo { get; set; }
        public string Nombre_Articulo { get; set; }
        public string Descripcion_Articulo { get; set; }
        public bool Activo { get; set; }

        public virtual PrecioModel? Precio { get; set; }

        public int Id_Categoria { get; set; }
        [ForeignKey("Id_Categoria")]
        public virtual CategoriaModel? Categoria { get; set; }
    }
}

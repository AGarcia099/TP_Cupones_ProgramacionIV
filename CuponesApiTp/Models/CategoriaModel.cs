using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CuponesApiTp.Models
{
    [Table("Categorias")]
    public class CategoriaModel
    {
        [Key]
        public int Id_Categoria { get; set; }
        public string Nombre { get; set; }
        public virtual ICollection<Cupon_CategoriaModel>? Cupones_Categorias { get; set; }
        public virtual ICollection<ArticuloModel>? Articulos { get; set; }
    }
}

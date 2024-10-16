using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CuponesApiTp.Models
{
    [Table("Cupones_Categorias")]
    public class Cupon_CategoriaModel
    {
        [Key]
        public int Id_Cupones_Categorias { get; set; }
        public int Id_Cupon { get; set; }
        public int Id_Categoria { get; set; }

        [ForeignKey("Id_Cupon")]
        public virtual CuponModel? Cupon { get; set; }
        [ForeignKey("Id_Categoria")]
        public virtual CategoriaModel? Categoria { get; set; }
    }
}

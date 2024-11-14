using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CuponesApiTp.Models
{
    [Table("Precios")]
    public class PrecioModel
    {
        [Key]
        public int Id_Precio { get; set; }
        public int Id_Articulo { get; set; }
        public decimal Precio { get; set; }

        [ForeignKey("Id_Articulo")]
        public virtual ArticuloModel? Articulo { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CuponesApiTp.Models
{
    [Table("Cupones_Detalle")]
    public class Cupones_DetalleModel
    {
        [Key]
        public int Id_Cupon { get; set; }
        [Key]
        public int Id_Articulo { get; set; }
        public int Cantidad { get; set; }
    }
}

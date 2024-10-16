using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CuponesApiTp.Models
{
    [Table("Tipo_Cupon")]
    public class Tipo_CuponModel
    {
        [Key]
        public int Id_Tipo_Cupon { get; set; }
        public string Nombre { get; set; }
    }
}

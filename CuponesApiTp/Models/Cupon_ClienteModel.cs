using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CuponesApiTp.Models
{
    [Table("Cupones_Clientes")]
    public class Cupon_ClienteModel
    {
        [ForeignKey("Id_Cupon")]
        public int Id_Cupon { get; set; }
        [Key]
        public string NroCupon { get; set; }
        public DateTime? FechaAsignado { get; set; }
        public string CodCliente { get; set; }
        public virtual CuponModel? Cupon { get; set; }
    }
}

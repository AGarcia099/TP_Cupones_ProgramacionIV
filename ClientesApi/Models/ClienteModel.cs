using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientesApi.Models
{
    [Table("Clientes")]
    public class ClienteModel
    {
        [Key]
        public string CodCliente { get; set; }
        public string Nombre_Cliente { get; set; }
        public string Apellido_Cliente { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }

    }
}

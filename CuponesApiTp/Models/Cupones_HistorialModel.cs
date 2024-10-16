﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CuponesApiTp.Models
{
    [Table("Cupones_Historial")]
    public class Cupones_HistorialModel
    {
        [Key]
        public int Id_Cupon { get; set; }
        [Key]
        public string NroCupon { get; set; }
        public DateTime FechaUso { get; set; }
        public string CodCliente { get; set; }
    }
}
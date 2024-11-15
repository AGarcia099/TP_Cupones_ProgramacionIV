namespace ClientesApi.Models.DTO
{
    public class CuponDto
    {
        public int Id_Cupon { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PorcentajeDTO { get; set; }
        public decimal ImportePromo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
    }
}

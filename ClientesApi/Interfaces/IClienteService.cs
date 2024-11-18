using ClientesApi.Models;
using ClientesApi.Models.DTO;

namespace ClientesApi.Interfaces
{
    public interface IClienteService
    {
        Task<string> SolicitarCupon(ClienteDto clienteDto);
        Task<string> QuemadoCupon(QuemarCuponDto quemarCuponDto);
        Task<IEnumerable<CuponDto>> ObtenerCuponesActivos(string codCliente);
    }
}

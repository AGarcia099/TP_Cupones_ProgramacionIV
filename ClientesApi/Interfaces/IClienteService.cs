using ClientesApi.Models;
using ClientesApi.Models.DTO;

namespace ClientesApi.Interfaces
{
    public interface IClienteService
    {
        Task<string> SolicitarCupon(ClienteDto clienteDto);
        Task<string> QuemadoCupon(QuemarCuponDto quemarCuponDto);
        Task<ClienteModel> CrearCliente(ClienteModel clienteModel);
        Task<bool> EliminarCliente(string codCliente);
        Task<ClienteModel> ModificarCliente(ClienteModel clienteModel);
        Task<List<ClienteModel>> ObtenerTodosLosClientes();
        Task<ClienteModel> ObtenerClientePorCodCliente(string codCliente);
        Task<IEnumerable<CuponDto>> ObtenerCuponesActivos(string codCliente);
    }
}

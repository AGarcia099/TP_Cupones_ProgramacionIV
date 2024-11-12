using ClientesApi.Data;
using ClientesApi.Interfaces;
using ClientesApi.Models;
using ClientesApi.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;


namespace ClientesApi.Services
{
    public class ClienteService : IClienteService
    {
        private readonly DataBaseContext _context;

        public ClienteService(DataBaseContext dataBaseContext)
        {
            _context = dataBaseContext;
        }
        //5000
        //7024
        public async Task<string> SolicitarCupon(ClienteDto clienteDto)
        {
            try
            {
                var jsonCliente = JsonConvert.SerializeObject(clienteDto);
                var contenido = new StringContent(jsonCliente, Encoding.UTF8, "application/json");
                var cliente = new HttpClient();
                var respuesta = await cliente.PostAsync("https://localhost:7024/api/SolicitudCupones/SolicitarCupon", contenido);

                if (respuesta.IsSuccessStatusCode)
                {
                    var mensaje = await respuesta.Content.ReadAsStringAsync();
                    return mensaje;
                }
                else
                {
                    var error = await respuesta.Content.ReadAsStringAsync();
                    throw new Exception($"{error}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<ClienteModel> CrearCliente(ClienteModel clienteModel)
        {
            _context.Clientes.Add(clienteModel);
            await _context.SaveChangesAsync();
            return clienteModel;
        }

        public async Task<bool> EliminarCliente(string codCliente)
        {
            // Buscar el cliente en la base de datos
            var cliente = await _context.Clientes.FindAsync(codCliente);

            if (cliente == null)
            {
                return false; // Cliente no encontrado
            }

            // Si existe, eliminarlo
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ClienteModel> ModificarCliente(ClienteModel clienteModel)
        {
            var clienteExistente = await _context.Clientes.FindAsync(clienteModel.CodCliente);

            if (clienteExistente == null)
            {
                return null; // Cliente no encontrado
            }

            // Actualizar los datos del cliente existente
            clienteExistente.Nombre_Cliente = clienteModel.Nombre_Cliente;
            clienteExistente.Apellido_Cliente = clienteModel.Apellido_Cliente;
            clienteExistente.Direccion = clienteModel.Direccion;
            clienteExistente.Email = clienteModel.Email;

            await _context.SaveChangesAsync();
            return clienteExistente;
        }

        public async Task<List<ClienteModel>> ObtenerTodosLosClientes()
        {
            return await _context.Clientes.ToListAsync(); // Obtener todos los clientes de la base de datos
        }

        public async Task<ClienteModel> ObtenerClientePorCodCliente(string codCliente)
        {
            return await _context.Clientes.FindAsync(codCliente); // Buscar cliente por CodCliente
        }
    }
}

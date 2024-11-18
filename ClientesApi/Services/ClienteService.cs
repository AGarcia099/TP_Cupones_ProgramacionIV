using ClientesApi.Data;
using ClientesApi.Interfaces;
using ClientesApi.Models;
using ClientesApi.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
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
                    return await respuesta.Content.ReadAsStringAsync();
                }

                if (respuesta.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var error = await respuesta.Content.ReadAsStringAsync();
                    throw new ArgumentException($"Solicitud inválida: {error}");
                }

                if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException("El cliente o el cupon no fueron encontrados.");
                }

                throw new HttpRequestException($"Error al procesar la solicitud: {respuesta.ReasonPhrase}.");
            }

            catch (Exception ex) when (ex is ArgumentException || ex is KeyNotFoundException || ex is HttpRequestException)
            {
                throw;
            }

            catch (Exception ex)
            {
                throw new Exception($"Error en el servicio de solicitud de cupones: {ex.Message}", ex);
            }
        }

        public async Task<string> QuemadoCupon(QuemarCuponDto quemarCuponDto)
        {
            try
            {
                var jsonPayload = JsonConvert.SerializeObject(quemarCuponDto);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                using var httpClient = new HttpClient();
                var response = await httpClient.PostAsync("https://localhost:7024/api/SolicitudCupones/QuemadoCupon", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al utilizar el cupon: {error}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud para quemar el cupón: {ex.Message}", ex);
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

        public async Task<IEnumerable<CuponDto>> ObtenerCuponesActivos(string codCliente)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"https://localhost:7024/api/Cupones/Cliente/{codCliente}");

                if (response.IsSuccessStatusCode)
                {
                    var cupones = await response.Content.ReadFromJsonAsync<IEnumerable<CuponDto>>();
                    return cupones ?? new List<CuponDto>();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException($"No se encontraron cupones para el cliente con código: {codCliente}.");
                }

                throw new HttpRequestException($"Error al llamar al servicio externo: {response.ReasonPhrase}.");
            }
        }
    }
}

using ClientesApi.Data;
using ClientesApi.Interfaces;
using ClientesApi.Models;
using ClientesApi.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ClientesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly DataBaseContext _context;

        public ClienteController(IClienteService clienteService, DataBaseContext dataBaseContext)
        {
            _clienteService = clienteService;
            _context = dataBaseContext;
        }

        [HttpGet("verificarCliente/{codCliente}")]
        [ApiExplorerSettings(IgnoreApi = true)]  // Ignora este endpoint en Swagger
        public async Task<IActionResult> VerificarCliente(string codCliente)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.CodCliente == codCliente);

            if (cliente == null)
            {
                return NotFound(new { Mensaje = "Cliente no encontrado" });
            }

            return Ok(new { Mensaje = "Cliente encontrado" });
        }

        [HttpPost("CrearCliente")]
        public async Task<IActionResult> CrearCliente([FromBody] ClienteModel clienteModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clienteModel.CodCliente))
                {
                    Log.Error("El CodCliente no puede estar vacío");
                    return BadRequest("El CodCliente no puede estar vacío");
                }

                var clienteExistente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.CodCliente == clienteModel.CodCliente);

                if (clienteExistente != null)
                {
                    Log.Error("No se puede registrar clientes con un mismo CodCliente");
                    return BadRequest($"Ya existe un cliente con el código de cliente: {clienteModel.CodCliente}");
                }

                _context.Clientes.Add(clienteModel);
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint CrearCliente");
                return Ok($"Cliente creado correctamente con CodCliente: {clienteModel.CodCliente}");
            }
            catch (Exception ex)
            {
                Log.Error($"Ocurrio un problema en CrearCliente. Error: {ex.Message}");
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("EliminarCliente/{codCliente}")]
        public async Task<IActionResult> EliminarCliente(string codCliente)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(codCliente);

                if (cliente == null)
                {
                    Log.Error($"No hay cliente con CodCliente '{codCliente}'");
                    return NotFound($"Cliente con CodCliente: {codCliente} no encontrado.");
                }

                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint EliminarCliente");
                return Ok($"Cliente con CodCliente: {codCliente} eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Log.Error($"Ocurrio un problema en EliminarCliente. Error: {ex.Message}");
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut("ModificarCliente/{codCliente}")]
        public async Task<IActionResult> ModificarCliente(string codCliente, [FromBody] ClienteModel clienteModel)
        {
            try
            {
                var clienteExistente = await _context.Clientes.FindAsync(codCliente);

                if (clienteExistente == null)
                {
                    Log.Error($"No hay cliente con CodCliente {codCliente}");
                    return NotFound($"Cliente con CodCliente: {codCliente} no encontrado.");
                }

                clienteExistente.Nombre_Cliente = clienteModel.Nombre_Cliente;
                clienteExistente.Apellido_Cliente = clienteModel.Apellido_Cliente;
                clienteExistente.Direccion = clienteModel.Direccion;
                clienteExistente.Email = clienteModel.Email;

                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint ModificarCliente");
                return Ok($"Cliente con CodCliente: {clienteExistente.CodCliente} modificado correctamente.");
            }
            catch (Exception ex)
            {
                Log.Error($"Ocurrio un problema en ModificarCliente. Error: {ex.Message}");
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("ObtenerTodosLosClientes")]
        public async Task<IActionResult> ObtenerTodosLosClientes()
        {
            try
            {
                var clientes = await _context.Clientes.ToListAsync();

                if (!clientes.Any())
                {
                    Log.Error("No se encontraron clientes registrados");
                    return NotFound("No hay clientes registrados.");
                }

                Log.Information("Se llamo al endpoint ObtenerTodosLosClientes");
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                Log.Error($"Ocurrio un problema en ObtenerTodosLosClientes. Error: {ex.Message}");
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("ObtenerClientePorCodCliente/{codCliente}")]
        public async Task<IActionResult> ObtenerClientePorCodCliente(string codCliente)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(codCliente);

                if (cliente == null)
                {
                    Log.Error($"No se encontro cliente con CodCliente '{codCliente}'");
                    return NotFound($"Cliente con CodCliente {codCliente} no encontrado.");
                }

                Log.Information("Se llamo al endpoint ObtenerClientesPorCodCliente");
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                Log.Error($"Ocurrio un problema en ObtenerClientesPorCodCliente. Error: {ex.Message}");
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("ReclamarCupon")]
        public async Task<IActionResult> ReclamarCupon([FromBody] ClienteDto clienteDto)
        {
            try
            {
                var respuesta = await _clienteService.SolicitarCupon(clienteDto);
                return Ok(respuesta);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Ocurrió un error inesperado.", detalle = ex.Message });
            }
        }

        [HttpPost("UsarCupon")]
        public async Task<IActionResult> UsarCupon([FromBody] QuemarCuponDto quemarCuponDto)
        {
            try
            {
                var resultado = await _clienteService.QuemadoCupon(quemarCuponDto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message});
            }
        }

        [HttpGet("ObtenerCuponesActivos/{codCliente}")]
        public async Task<IActionResult> ObtenerCuponesActivos(string codCliente)
        {
            try
            {
                var cupones = await _clienteService.ObtenerCuponesActivos(codCliente);

                if(cupones == null || !cupones.Any())
                    return NotFound($"No se encontraron cupones activos para el cliente con código: {codCliente}");

                return Ok(cupones);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, $"Problema al comunicarse con el servicio: {ex.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error inesperado: {ex.Message}");
            }
        }
    }
}

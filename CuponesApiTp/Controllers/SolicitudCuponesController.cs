using CuponesApiTp.Data;
using CuponesApiTp.Interfaces;
using CuponesApiTp.Models;
using CuponesApiTp.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CuponesApiTp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudCuponesController : ControllerBase
    {
        private readonly DataBaseContext _context;
        private readonly ICuponesServices _cuponesServices;
        private readonly ISendEmailService _sendEmailService;

        public SolicitudCuponesController(DataBaseContext context, ICuponesServices cuponesServices, ISendEmailService sendEmailService)
        {
            _context = context;
            _cuponesServices = cuponesServices;
            _sendEmailService = sendEmailService;
        }

        [HttpPost("SolicitarCupon")]
        public async Task<IActionResult> SolicitarCupon(ClienteDto clienteDto)
        {
            try
            {
                if (clienteDto.CodCliente.IsNullOrEmpty())
                    throw new Exception("El CodCliente no puede estar vacio");

                var clienteExistenteResponse = await VerificarClienteExistente(clienteDto.CodCliente);

                if (!clienteExistenteResponse.IsSuccessStatusCode)
                    throw new Exception("No existe un cliente con el CodCliente proporcionado");

                string nroCupon = await _cuponesServices.GenerarNroCupon();

                Cupon_ClienteModel cupon_Cliente = new Cupon_ClienteModel()
                {
                    Id_Cupon = clienteDto.Id_Cupon,
                    CodCliente = clienteDto.CodCliente,
                    FechaAsignado = DateTime.Now,
                    NroCupon = nroCupon
                };

                _context.Cupones_Clientes.Add(cupon_Cliente);
                await _context.SaveChangesAsync();

                var subject = "Numero de cupon asignado";
                var messageBody = $"Su numero de cupon es: {nroCupon}";
                await _sendEmailService.EnviarEmailCliente(clienteDto.Email, nroCupon, subject, messageBody);

                return Ok(new
                {
                    Mensaje = $"Se ha asignado el cupon {clienteDto.Id_Cupon} al cliente {clienteDto.CodCliente}",
                    NroCupon = nroCupon
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("QuemadoCupon")]
        public async Task<IActionResult> QuemadoCupon([FromBody] QuemarCuponDto quemarCuponDto)
        {
            try
            {
                if (string.IsNullOrEmpty(quemarCuponDto.NroCupon))
                    throw new Exception("El número de cupón no puede estar vacío");

                if (string.IsNullOrEmpty(quemarCuponDto.Email))
                    throw new Exception("El correo electrónico no puede estar vacío");

                var cuponCliente = await _context.Cupones_Clientes
                    .FirstOrDefaultAsync(c => c.NroCupon == quemarCuponDto.NroCupon);

                if (cuponCliente == null)
                    throw new Exception("El cupón no existe o ya ha sido utilizado");

                var historialRegistro = new Cupones_HistorialModel
                {
                    NroCupon = cuponCliente.NroCupon,
                    CodCliente = cuponCliente.CodCliente,
                    Id_Cupon = cuponCliente.Id_Cupon,
                    FechaUso = DateTime.Now
                };

                _context.Cupones_Historial.Add(historialRegistro);
                _context.Cupones_Clientes.Remove(cuponCliente);
                await _context.SaveChangesAsync();

                var subject = "Número de cupón utilizado";
                var messageBody = $"Ha utilizado el cupón: {quemarCuponDto.NroCupon}.";
                await _sendEmailService.EnviarEmailCliente(quemarCuponDto.Email, quemarCuponDto.NroCupon, subject, messageBody);

                return Ok(new { Mensaje = "El cupón fue utilizado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message});
            }
        }

        private async Task<HttpResponseMessage> VerificarClienteExistente(string codCliente)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7107/api/Cliente/"); 
                var response = await client.GetAsync($"verificarCliente/{codCliente}"); 
                return response;
            }
        }
    }
}
﻿using CuponesApiTp.Data;
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
        public async Task<IActionResult> QuemarCupon([FromBody] QuemarCuponDto quemarCuponDto)
        {
            try
            {
                // Validar que el número de cupón no esté vacío
                if (string.IsNullOrEmpty(quemarCuponDto.NroCupon))
                    throw new Exception("El número de cupón no puede estar vacío");

                // Buscar el cupón en Cupones_Clientes
                var cuponCliente = await _context.Cupones_Clientes
                    .FirstOrDefaultAsync(c => c.NroCupon == quemarCuponDto.NroCupon);

                if (cuponCliente == null)
                    throw new Exception("El cupón no existe o ya ha sido utilizado");

                // Insertar registro en Cupones_Historial
                var historialRegistro = new Cupones_HistorialModel
                {
                    NroCupon = cuponCliente.NroCupon,
                    CodCliente = cuponCliente.CodCliente,
                    Id_Cupon = cuponCliente.Id_Cupon,
                    FechaUso = DateTime.Now
                };

                _context.Cupones_Historial.Add(historialRegistro);

                // Eliminar el registro de Cupones_Clientes
                _context.Cupones_Clientes.Remove(cuponCliente);

                await _context.SaveChangesAsync();

                var clienteDto = new ClienteDto
                {
                    Email = "programacioniv.agus@gmail.com",
                    CodCliente = cuponCliente.CodCliente
                };

                var subject = "Numero de cupon usado";
                var messageBody = $"Ha usado el cupon: {quemarCuponDto.NroCupon}.";
                await _sendEmailService.EnviarEmailCliente(clienteDto.Email, quemarCuponDto.NroCupon, subject, messageBody);

                return Ok(new { Mensaje = "El cupón fue utilizado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
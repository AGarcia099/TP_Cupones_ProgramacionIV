﻿using ClientesApi.Interfaces;
using ClientesApi.Models;
using ClientesApi.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpPost]
        public async Task<IActionResult> EnviarSolicitudACupones([FromBody] ClienteDto clienteDto)
        {
            try
            {
                var respuesta = await _clienteService.SolicitarCupon(clienteDto);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("CrearCliente")]
        public async Task<IActionResult> CrearCliente([FromBody] ClienteModel clienteModel)
        {
            try
            {
                var clienteCreado = await _clienteService.CrearCliente(clienteModel);
                return Ok($"Cliente creado correctamente con CodCliente: {clienteCreado.CodCliente}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("EliminarCliente/{codCliente}")]
        public async Task<IActionResult> EliminarCliente(string codCliente)
        {
            try
            {
                bool clienteEliminado = await _clienteService.EliminarCliente(codCliente);

                if (clienteEliminado)
                {
                    return Ok($"Cliente con CodCliente: {codCliente} eliminado correctamente.");
                }
                else
                {
                    return NotFound($"Cliente con CodCliente: {codCliente} no encontrado.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut("ModificarCliente")]
        public async Task<IActionResult> ModificarCliente([FromBody] ClienteModel clienteModel)
        {
            try
            {
                var clienteModificado = await _clienteService.ModificarCliente(clienteModel);

                if (clienteModificado != null)
                {
                    return Ok($"Cliente con CodCliente: {clienteModificado.CodCliente} modificado correctamente.");
                }
                else
                {
                    return NotFound($"Cliente con CodCliente: {clienteModel.CodCliente} no encontrado.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("ObtenerTodosLosClientes")]
        public async Task<IActionResult> ObtenerTodosLosClientes()
        {
            try
            {
                var clientes = await _clienteService.ObtenerTodosLosClientes();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("ObtenerClientePorCodCliente/{codCliente}")]
        public async Task<IActionResult> ObtenerClientePorCodCliente(string codCliente)
        {
            try
            {
                var cliente = await _clienteService.ObtenerClientePorCodCliente(codCliente);

                if (cliente == null)
                {
                    return NotFound($"Cliente con CodCliente {codCliente} no encontrado.");
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}

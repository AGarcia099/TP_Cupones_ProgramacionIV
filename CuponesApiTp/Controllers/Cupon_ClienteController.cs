using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CuponesApiTp.Data;
using CuponesApiTp.Models;
using Serilog;
using CuponesApiTp.Services;
using CuponesApiTp.Interfaces;
using System.Text.RegularExpressions;

namespace CuponesApiTp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Cupon_ClienteController : ControllerBase
    {
        private readonly DataBaseContext _context;
        public Cupon_ClienteController(DataBaseContext context )
        {
            _context = context;
        }

        // GET: api/Cupon_Cliente
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cupon_ClienteModel>>> GetCupones_Clientes()
        {
            try
            {
                var cuponesClientes = await _context.Cupones_Clientes.ToListAsync();
                
                if(cuponesClientes == null || !cuponesClientes.Any())
                {
                    Log.Error("No hay cupon_cliente");
                    return NotFound("No hay cupon_clientes.");
                }

                Log.Information("Se llamo al endpoint GetCupones_Clientes");
                return Ok(cuponesClientes);
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema en GetCupones_Clientes. Error: {ex.Message}");
                return BadRequest($"Hubo un problema al obtener los cupon_clientes. Error: {ex.Message}");
            }
        }

        // GET: api/Cupon_Cliente/5
        [HttpGet("{idCupon}")]
        public async Task<ActionResult<IEnumerable<Cupon_ClienteModel>>> GetCuponesByIdCupon(int idCupon)
        {
            try
            {
                var cupones = await _context.Cupones_Clientes
                    .Where(c => c.Id_Cupon == idCupon)
                    .ToListAsync();

                if (cupones == null || !cupones.Any())
                {
                    Log.Error($"No se encontraron registros de cupon_cliente con Id_Cupon '{idCupon}'.");
                    return NotFound($"No se encontraron cupones_clientes con Id_Cupon '{idCupon}'.");
                }

                Log.Information($"Se llamó al endpoint GetCuponesByIdCupon");
                return Ok(cupones);
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema en GetCuponesByIdCupon. Error: {ex.Message}");
                return BadRequest($"Hubo un problema al obtener los cupones_clientes. Error: {ex.Message}");
            }
        }

        // PUT: api/Cupon_Cliente/5
        [HttpPut("{nroCupon}")]
        public async Task<IActionResult> PutCupon_ClienteModel(string nroCupon, Cupon_ClienteModel cupon_ClienteModel)
        {
            if (nroCupon != cupon_ClienteModel.NroCupon)
            {
                Log.Error("Ningun NrCupon coincide con ese id");
                return BadRequest("El ID proporcionado no coincide con el NroCupon del cupon_cliente.");
            }

            var regex = new Regex(@"^\d{3}-\d{3}-\d{3}$");
            if (!regex.IsMatch(cupon_ClienteModel.NroCupon))
            {
                Log.Error("El NroCupon no cumple con el formato requerido.");
                return BadRequest("El formato de NroCupon no es válido. Debe ser del tipo 123-456-789.");
            }

            var clienteResponse = await VerificarClienteExistente(cupon_ClienteModel.CodCliente);
            if (!clienteResponse.IsSuccessStatusCode)
            {
                Log.Error($"No se encontró el cliente con CodCliente '{cupon_ClienteModel.CodCliente}'.");
                return BadRequest($"El cliente con CodCliente '{cupon_ClienteModel.CodCliente}' no existe.");
            }

            _context.Entry(cupon_ClienteModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint PutCupon_Cliente");
                return Ok($"Los datos del cupon_cliente con ID '{nroCupon}' fueron modificados correctamente.");
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema en PutCupon_Cliente. Error: {ex.Message}");
                return BadRequest($"Ocurrió un error al intentar modificar el cupon_cliente. Error: {ex.Message}");
            }
        }

        // POST: api/Cupon_Cliente
        [HttpPost]
        public async Task<ActionResult<Cupon_ClienteModel>> PostCupon_ClienteModel(Cupon_ClienteModel cupon_ClienteModel)
        {
            cupon_ClienteModel.FechaAsignado = DateTime.Now;

            var regex = new System.Text.RegularExpressions.Regex(@"^\d{3}-\d{3}-\d{3}$");
            if (!regex.IsMatch(cupon_ClienteModel.NroCupon))
            {
                Log.Error("El NroCupon no cumple con el formato requerido (123-456-789).");
                return BadRequest("El formato del NroCupon es inválido. Debe ser 123-456-789.");
            }

            if (Cupon_ClienteModelExists(cupon_ClienteModel.NroCupon))
            {
                Log.Error("El NroCupon no puede repetirse");
                return BadRequest($"El cupon_cliente con NroCupon '{cupon_ClienteModel.NroCupon}' ya existe.");
            }

            var cuponExiste = await _context.Cupones.AnyAsync(c => c.Id_Cupon == cupon_ClienteModel.Id_Cupon);
            if (!cuponExiste)
            {
                Log.Error($"El Id_Cupon '{cupon_ClienteModel.Id_Cupon}' no existe en la tabla Cupones.");
                return NotFound($"El cupon con Id_Cupon '{cupon_ClienteModel.Id_Cupon}' no existe.");
            }

            var clienteResponse = await VerificarClienteExistente(cupon_ClienteModel.CodCliente);
            if (!clienteResponse.IsSuccessStatusCode)
            {
                Log.Error($"El CodCliente '{cupon_ClienteModel.CodCliente}' no existe en la API de Clientes.");
                return NotFound($"El cliente con CodCliente '{cupon_ClienteModel.CodCliente}' no existe.");
            }

            _context.Cupones_Clientes.Add(cupon_ClienteModel);
            try
            {
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint PostCupon_Cliente");
                return Ok("Se dio de alta el registro en Cupon_Cliente");
            }
            catch (Exception ex)
            {
                Log.Error($"Ocurrio un problema en PostCupon_Cliente. Error: {ex.Message}");
                return BadRequest($"Ocurrió un error inesperado: {ex.Message}");
            }
        }

        // DELETE: api/Cupon_Cliente/5
        [HttpDelete("{nroCupon}")]
        public async Task<IActionResult> DeleteCupon_ClienteModel(string nroCupon)
        {
            try
            {
                var cupon_ClienteModel = await _context.Cupones_Clientes.FindAsync(nroCupon);

                if (cupon_ClienteModel == null)
                {
                    Log.Error("No existe el cupon_cliente con ese NroCupon");
                    return NotFound($"El cupon_cliente con NroCupon '{nroCupon}' no existe.");
                }

                _context.Cupones_Clientes.Remove(cupon_ClienteModel);
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint DeleteCupon_Cliente");
                return Ok($"El cupon_cliente con NroCupon '{nroCupon}' fue eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Log.Error($"Occurrio un error en DeleteCupon_Cliente. Error: {ex.Message}");
                return BadRequest($"Ocurrió un error al intentar eliminar el cupon_cliente: {ex.Message}");
            }
        }

        private bool Cupon_ClienteModelExists(string id)
        {
            return _context.Cupones_Clientes.Any(e => e.NroCupon == id);
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

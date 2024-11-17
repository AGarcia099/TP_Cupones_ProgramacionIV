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

namespace CuponesApiTp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Cupon_ClienteController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public Cupon_ClienteController(DataBaseContext context)
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
        [HttpGet("{id}")]
        public async Task<ActionResult<Cupon_ClienteModel>> GetCupon_ClienteModel(string id)
        {
            try
            {
                var cupon_ClienteModel = await _context.Cupones_Clientes.FindAsync(id);

                if (cupon_ClienteModel == null)
                {
                    Log.Error("No se encontro ningun cupon_cliente con ese id");
                    return NotFound($"El cupon_cliente con el ID '{id}' no existe.");
                }

                Log.Information("Se llamo al endpoint de GetCupon_Cliente por id");
                return Ok(cupon_ClienteModel);
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema en GetCupon_Cliente por id. Error: {ex.Message}");
                return BadRequest($"Hubo un problema al obtener el cupon_cliente. Error: {ex.Message}");
            }
        }

        // PUT: api/Cupon_Cliente/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCupon_ClienteModel(string id, Cupon_ClienteModel cupon_ClienteModel)
        {
            if (id != cupon_ClienteModel.NroCupon)
            {
                Log.Error("Ningun NrCupon coincide con ese id");
                return BadRequest("El ID proporcionado no coincide con el NroCupon del cupon_cliente.");
            }

            _context.Entry(cupon_ClienteModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint PutCupon_Cliente");
                return Ok($"Los datos del cupon_cliente con ID '{id}' fueron modificados correctamente.");
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema en PutCupon_Cliente. Error: {ex.Message}");
                return BadRequest($"Ocurrió un error al intentar modificar el cupon_cliente. Error: {ex.Message}");
            }
        }

        // POST: api/Cupon_Cliente
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cupon_ClienteModel>> PostCupon_ClienteModel(Cupon_ClienteModel cupon_ClienteModel)
        {
            cupon_ClienteModel.FechaAsignado = DateTime.Now;

            if (Cupon_ClienteModelExists(cupon_ClienteModel.NroCupon))
            {
                Log.Error("El NroCupon no puede repetirse");
                return BadRequest($"El cupon_cliente con NroCupon '{cupon_ClienteModel.NroCupon}' ya existe.");
            }

            _context.Cupones_Clientes.Add(cupon_ClienteModel);
            try
            {
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint PostCupon_Cliente");
                return Ok("Se dio de alta el registro en Cupon_Cliente");
            }
            catch(Exception ex)
            {
                Log.Error($"Ocurrio un problema en PostCupon_Cliente. Error: {ex.Message}");
                return BadRequest($"Ocurrió un error inesperado: {ex.Message}");
            }
        }

        // DELETE: api/Cupon_Cliente/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCupon_ClienteModel(string id)
        {
            try
            {
                var cupon_ClienteModel = await _context.Cupones_Clientes.FindAsync(id);
                
                if (cupon_ClienteModel == null)
                {
                    Log.Error("No existe el cupon_cliente con ese NroCupon");
                    return NotFound($"El cupon_cliente con NroCupon '{id}' no existe.");
                }

                _context.Cupones_Clientes.Remove(cupon_ClienteModel);
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint DeleteCupon_Cliente");
                return Ok($"El cupon_cliente con NroCupon '{id}' fue eliminado correctamente.");
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
    }
}

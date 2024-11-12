using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CuponesApiTp.Data;
using CuponesApiTp.Models;

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
                    return NotFound("No hay cupon_clientes.");

                return Ok(cuponesClientes);
            }
            catch (Exception ex)
            {
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
                    return NotFound($"El cupon_cliente con el ID '{id}' no existe.");

                return Ok(cupon_ClienteModel);
            }
            catch (Exception ex)
            {
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
                return BadRequest("El ID proporcionado no coincide con el NroCupon del cupon_cliente.");
            }

            _context.Entry(cupon_ClienteModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                return Ok($"Los datos del cupon_cliente con ID '{id}' fueron modificados correctamente.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Cupon_ClienteModelExists(id))
                {
                    return NotFound($"No existe ningun cupon_cliente con el ID '{id}'");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al intentar modificar el cupon_cliente. Error: {ex.Message}");
            }
        }

        // POST: api/Cupon_Cliente
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cupon_ClienteModel>> PostCupon_ClienteModel(Cupon_ClienteModel cupon_ClienteModel)
        {
            cupon_ClienteModel.FechaAsignado = DateTime.Now;

            _context.Cupones_Clientes.Add(cupon_ClienteModel);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Cupon_ClienteModelExists(cupon_ClienteModel.NroCupon))
                {
                    return Conflict($"El cupon_cliente con NroCupon '{cupon_ClienteModel.NroCupon}' ya existe.");
                }
                else
                {
                    return Ok("Se dio de alta el registro en Cupon_Cliente");
                }
            }
            catch(Exception ex)
            {
                return BadRequest($"Ocurrió un error inesperado: {ex.Message}");
            }

            return CreatedAtAction("GetCupon_ClienteModel", new { id = cupon_ClienteModel.NroCupon }, cupon_ClienteModel);
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
                    return NotFound($"El cupon_cliente con NroCupon '{id}' no existe.");
                }

                _context.Cupones_Clientes.Remove(cupon_ClienteModel);
                await _context.SaveChangesAsync();

                return Ok($"El cupon_cliente con NroCupon '{id}' fue eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al intentar eliminar el cupon_cliente: {ex.Message}");
            }
        }

        private bool Cupon_ClienteModelExists(string id)
        {
            return _context.Cupones_Clientes.Any(e => e.NroCupon == id);
        }
    }
}

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
    public class Cupones_HistorialController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public Cupones_HistorialController(DataBaseContext context)
        {
            _context = context;
        }

        // GET: api/Cupones_Historial
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cupones_HistorialModel>>> GetCupones_Historial()
        {
            try
            {
                var cuponesHistorial = await _context.Cupones_Historial.ToListAsync();
                
                if(cuponesHistorial == null || !cuponesHistorial.Any())
                {
                    Log.Error("No hay registros de Cupones_Historial");
                    return NotFound("No hay Cupones_Historial");
                }
                Log.Information("Se llamo al endpoint GetCupones_Historial");
                return Ok(cuponesHistorial);
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema en GetCupones_Historial. Error: {ex.Message}");
                return BadRequest($"Hubo un problema al obtener los Cupones_Historial. Error: {ex.Message}");
            }
        }

        // GET: api/Cupones_Historial/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cupones_HistorialModel>> GetCupones_HistorialModel(int id)
        {
            try
            {
                var cuponesHistorialModel = await _context.Cupones_Historial
                    .Where(ch => ch.Id_Cupon == id)
                    .FirstOrDefaultAsync();

                if (cuponesHistorialModel == null)
                {
                    Log.Error($"No hay Cupon_Historial con Id_Cupon '{id}'");
                    return NotFound($"No hay ningun Cupon_Historial con el Id_Cupon '{id}'.");
                }

                Log.Information("Se llamo al endpoint GetCupones_Historial por Id_Cupon");
                return Ok(cuponesHistorialModel);
            }
            catch (Exception ex)
            {
                Log.Error($"Ocurrio un problema en GetCupones_Historial. Error: {ex.Message}");
                return BadRequest($"Hubo un problema al obtener el CuponHistorial con el Id_Cupon '{id}'. Error: {ex.Message}");
            }
        }

        // PUT: api/Cupones_Historial/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCupones_HistorialModel(int id, Cupones_HistorialModel cupones_HistorialModel)
        {
            if (id != cupones_HistorialModel.Id_Cupon)
            {
                Log.Error("El id ingresado no coincide con ningun Id_Cupon");
                return BadRequest("El ID proporcionado no coincide con el Id_Cupon del historial.");
            }

            _context.Entry(cupones_HistorialModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint PutCupones_Historial");
                return Ok($"Los datos del Cupon_Historial con Id_Cupon '{id}' fueron modificados correctamente.");
            }
            catch (Exception ex)
            {
                Log.Error($"Ocurrio un problema en PutCupones_Historial. Error: {ex.Message}");
                return BadRequest($"Hubo un problema al intentar actualizar el Cupon_Historial con Id_Cupon '{id}'. Error: {ex.Message}");
            }
        }

        // POST: api/Cupones_Historial
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cupones_HistorialModel>> PostCupones_HistorialModel(Cupones_HistorialModel cupones_HistorialModel)
        {
            if (string.IsNullOrEmpty(cupones_HistorialModel.NroCupon))
            {
                Log.Error("El NroCupon es obligatorio");
                return BadRequest("El NroCupon es obligatorio.");
            }

            if (string.IsNullOrEmpty(cupones_HistorialModel.CodCliente))
            {
                Log.Error("El CodCliente es obligatorio");
                return BadRequest("El CodCliente es obligatorio.");
            }

            _context.Cupones_Historial.Add(cupones_HistorialModel);
            try
            {
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint PostCupones_Historial");
                return CreatedAtAction("GetCupones_HistorialModel", new { id = cupones_HistorialModel.Id_Cupon }, cupones_HistorialModel);
            }
            catch (Exception ex)
            {
                Log.Error($"Ocurrio un problema en PostCupones_Historial. Error: {ex.Message}");
                return BadRequest($"Hubo un problema al intentar crear el Cupon_Historial. Error: {ex.Message}");
            }
        }

        // DELETE: api/Cupones_Historial/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCupones_HistorialModel(int id, string nroCupon)
        {
            try
            {
                var cupones_HistorialModel = await _context.Cupones_Historial
                    .FirstOrDefaultAsync(ch => ch.Id_Cupon == id && ch.NroCupon == nroCupon);

                if (cupones_HistorialModel == null)
                {
                    Log.Error("No hay un cupon_historial con ese Id_Cupon y NroCupon");
                    return NotFound($"No hay un Cupon_Historial con Id_Cupon '{id}' y NroCupon '{nroCupon}'.");
                }

                _context.Cupones_Historial.Remove(cupones_HistorialModel);
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint DeleteCupones_Historial");
                return Ok($"El Cupon_Historial con Id_Cupon '{id}' y NroCupon '{nroCupon}' fue eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                Log.Error($"Ocurrio un problema en DeleteCupones_Historial. Error: {ex.Message}");
                return BadRequest($"Hubo un problema al intentar eliminar el Cupon_Historial. Error: {ex.Message}");
            }
        }
    }
}

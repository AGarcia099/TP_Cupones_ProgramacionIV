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

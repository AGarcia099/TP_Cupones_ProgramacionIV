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
    public class Cupones_DetallesController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public Cupones_DetallesController(DataBaseContext context)
        {
            _context = context;
        }

        // GET: api/Cupones_Detalles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cupones_DetalleModel>>> GetCupones_Detalles()
        {
            try
            {
                var cuponesDetalles = await _context.Cupones_Detalle.ToListAsync();

                if(cuponesDetalles == null || !cuponesDetalles.Any())
                    return NotFound("No existen Cupones_Detalle.");

                return Ok(cuponesDetalles);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un problema al obtener los cupones detalle. Error: {ex.Message}");
            }
        }

        // GET: api/Cupones_Detalles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Cupones_DetalleModel>>> GetCupones_DetallesModel(int id)
        {
            try
            {
                var cupones_DetallesModel = await _context.Cupones_Detalle.Where(cd => cd.Id_Cupon == id).ToListAsync();

                if (cupones_DetallesModel == null || !cupones_DetallesModel.Any())
                    return NotFound($"El Id_Cupon '{id}' no existe");

                return Ok(cupones_DetallesModel);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un problema al obtener los cupones detalle. Error: {ex.Message}");
            }

        }

        // PUT: api/Cupones_Detalles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCupones_DetallesModel(int id, Cupones_DetalleModel cupones_DetallesModel)
        {
            if (id != cupones_DetallesModel.Id_Cupon)
            {
                return BadRequest("El ID proporcionado no coincide con el Id_Cupon del cupon_detalle.");
            }

            _context.Entry(cupones_DetallesModel).State = EntityState.Modified;

            try
            {
                if (cupones_DetallesModel.Id_Cupon <= 0)
                    return BadRequest("El Id_Cupon debe ser valido.");

                if (cupones_DetallesModel.Id_Articulo <= 0)
                    return BadRequest("El Id_Articulo debe ser valido.");

                var cuponExistente = await _context.Cupones.FindAsync(cupones_DetallesModel.Id_Cupon);
                if (cuponExistente == null)
                    return BadRequest($"El Id_Cupon '{cupones_DetallesModel.Id_Cupon}' no existe.");

                var articuloExistente = await _context.Articulos.FindAsync(cupones_DetallesModel.Id_Articulo);
                if (articuloExistente == null)
                    return BadRequest($"El Id_Articulo '{cupones_DetallesModel.Id_Articulo}' no existe.");

                await _context.SaveChangesAsync();

                return Ok($"Los datos del Id_Cupon '{id}' fueron modificados correctamente.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Cupones_DetallesModelExists(id))
                {
                    return NotFound($"El CuponDetalle con el Id_Cupon '{id}' no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un problema al intentar modificar los detalles del cupon. Error: {ex.Message}");
            }
        }

        // POST: api/Cupones_Detalles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cupones_DetalleModel>> PostCupones_DetallesModel(Cupones_DetalleModel cupones_DetallesModel)
        {
            try
            {
                if (cupones_DetallesModel.Id_Cupon <= 0)
                    return BadRequest("El Id_Cupon debe ser valido.");

                if (cupones_DetallesModel.Id_Articulo <= 0)
                    return BadRequest("El Id_Articulo debe ser valido.");

                var cuponExistente = await _context.Cupones.FindAsync(cupones_DetallesModel.Id_Cupon);
                if (cuponExistente == null)
                    return BadRequest($"El Id_Cupon '{cupones_DetallesModel.Id_Cupon}' no existe.");

                var articuloExistente = await _context.Articulos.FindAsync(cupones_DetallesModel.Id_Articulo);
                if (articuloExistente == null)
                    return BadRequest($"El Id_Articulo '{cupones_DetallesModel.Id_Articulo}' no existe.");

                _context.Cupones_Detalle.Add(cupones_DetallesModel);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCupones_DetallesModel", new { id = cupones_DetallesModel.Id_Cupon }, cupones_DetallesModel);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error inesperado. Error: {ex.Message}");
            }
        }

        // DELETE: api/Cupones_Detalles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCupones_DetallesModel(int id)
        {
            try
            {
                var cupones_DetallesModel = await _context.Cupones_Detalle
                    .Where(cd => cd.Id_Cupon == id)  
                    .FirstOrDefaultAsync();

                if (cupones_DetallesModel == null)
                {
                    return NotFound($"No existe un CuponDetalle con el Id_Cupon '{id}'.");
                }

                _context.Cupones_Detalle.Remove(cupones_DetallesModel);
                await _context.SaveChangesAsync();

                return Ok($"El CuponDetalle con Id_Cupon '{id}' fue eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un problema al intentar eliminar el CuponDetalle con el Id_Cupon '{id}'. Error: {ex.Message}");
            }
        }

        private bool Cupones_DetallesModelExists(int id)
        {
            return _context.Cupones_Detalle.Any(e => e.Id_Cupon == id);
        }
    }
}

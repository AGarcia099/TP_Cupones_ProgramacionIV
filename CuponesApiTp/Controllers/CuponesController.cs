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
    public class CuponesController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public CuponesController(DataBaseContext context)
        {
            _context = context;
        }

        // GET: api/Cupones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CuponModel>>> GetCupones()
        {
            try
            {
                var cupones =  await _context
                    .Cupones
                    .Include(c => c.Cupones_Categorias)
                        .ThenInclude(cc => cc.Categoria)
                    .Include(c => c.Tipo_Cupon)
                    .ToListAsync();

                if(cupones == null || !cupones.Any())
                    return NotFound("No hay cupones");

                return Ok(cupones);
            }
            catch(Exception ex)
            {
                return BadRequest($"Ocurrió un error al obtener los cupones: {ex.Message}");
            }
        }

        // GET: api/Cupones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CuponModel>> GetCuponModel(int id)
        {
            try
            {
                var cuponModel = await _context.Cupones
                    .Include(c => c.Cupones_Categorias)
                        .ThenInclude(cc => cc.Categoria)
                    .Include(c => c.Tipo_Cupon)
                    .FirstOrDefaultAsync(c => c.Id_Cupon == id);

                if (cuponModel == null)
                    return NotFound($"No existe un cupon con el Id_Cupon '{id}'.");

                return Ok(cuponModel);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al obtener el cupon con Id_Cupon {id}: {ex.Message}");
            }
        }

        // PUT: api/Cupones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCuponModel(int id, CuponModel cuponModel)
        {
            if (id != cuponModel.Id_Cupon)
            {
                return BadRequest($"El ID {id} no coincide");
            }

            if (!CuponModelExists(id))
            {
                return NotFound($"El cupon con Id_Cupon '{id}' no existe");
            }

            _context.Entry(cuponModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok($"El cupon con Id_Cupon '{id}' ha sido actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al intentar actualizar el cupon con Id_Cupon '{id}': {ex.Message}");
            }
        }

        // POST: api/Cupones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CuponModel>> PostCuponModel(CuponModel cuponModel)
        {
            if (string.IsNullOrEmpty(cuponModel.Nombre))
                return BadRequest("El nombre del cupon es obligatorio");

            if (cuponModel.FechaInicio >= cuponModel.FechaFin)
                return BadRequest("La fecha de inicio debe ser anterior a la fecha de finalización.");

            try
            {
                _context.Cupones.Add(cuponModel);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCuponModel", new { id = cuponModel.Id_Cupon }, cuponModel);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error inesperado al intentar crear el cupon: {ex.Message}");
            }
        }
            

        // DELETE: api/Cupones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCuponModel(int id)
        {
            try
            {
                var cuponModel = await _context.Cupones.FindAsync(id);
                
                if (cuponModel == null)
                {
                    return NotFound($"El cupon con Id_Cupon '{id}' no existe.");
                }

                _context.Cupones.Remove(cuponModel);
                await _context.SaveChangesAsync();

                return Ok($"El cupon con Id_Cupon '{id}' se elimino exitosamente.");
            }
            catch(Exception ex)
            {
                return BadRequest($"Ocurrió un error al intentar eliminar el cupon con Id_Cupon '{id}': {ex.Message}");
            }
        }

        private bool CuponModelExists(int id)
        {
            return _context.Cupones.Any(e => e.Id_Cupon == id);
        }
    }
}

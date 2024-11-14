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
                    .Include(c => c.Cupones_Categorias)!
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

        // GET: api/Cupones/Cliente/{codCliente}
        [HttpGet("Cliente/{codCliente}")]
        public async Task<ActionResult<IEnumerable<CuponModel>>> GetCuponesPorCliente(string codCliente)
        {
            try
            {
                var cupones = await _context.Cupones
                    .Where(c => _context.Cupones_Clientes.Any(cc => cc.CodCliente == codCliente && cc.Id_Cupon == c.Id_Cupon))
                    .Include(c => c.Cupones_Categorias)!
                        .ThenInclude(cc => cc.Categoria)
                    .Include(c => c.Tipo_Cupon)
                    .ToListAsync();

                if (cupones == null || !cupones.Any())
                    return NotFound($"No se encontraron cupones asociados con el código de cliente '{codCliente}'.");

                return Ok(cupones);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al obtener los cupones para el cliente con código '{codCliente}': {ex.Message}");
            }
        }

        // GET: api/Cupones/5
        [HttpGet("{id_Cupon}")]
        public async Task<ActionResult<CuponModel>> GetCuponModel(int id_Cupon)
        {
            try
            {
                var cuponModel = await _context.Cupones
                    .Include(c => c.Cupones_Categorias)!
                        .ThenInclude(cc => cc.Categoria)
                    .Include(c => c.Tipo_Cupon)
                    .FirstOrDefaultAsync(c => c.Id_Cupon == id_Cupon);

                if (cuponModel == null)
                    return NotFound($"No existe un cupon con el Id_Cupon '{id_Cupon}'.");

                return Ok(cuponModel);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al obtener el cupon con Id_Cupon {id_Cupon}: {ex.Message}");
            }
        }

        // PUT: api/Cupones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id_Cupon}")]
        public async Task<IActionResult> PutCuponModel(int id_Cupon, CuponModel cuponModel)
        {
            if (id_Cupon != cuponModel.Id_Cupon)
            {
                return BadRequest($"El ID {id_Cupon} no coincide");
            }

            if (!CuponModelExists(id_Cupon))
            {
                return NotFound($"El cupon con Id_Cupon '{id_Cupon}' no existe");
            }

            _context.Entry(cuponModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok($"El cupon con Id_Cupon '{id_Cupon}' ha sido actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al intentar actualizar el cupon con Id_Cupon '{id_Cupon}': {ex.Message}");
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
        [HttpDelete("{id_Cupon}")]
        public async Task<IActionResult> DeleteCuponModel(int id_Cupon)
        {
            try
            {
                var cuponModel = await _context.Cupones.FindAsync(id_Cupon);
                
                if (cuponModel == null)
                    return NotFound($"El cupon con Id_Cupon '{id_Cupon}' no existe.");

                if (!cuponModel.Activo)
                    return BadRequest("El cupon ya se encuentra inactivo.");

                cuponModel.Activo = false;
                _context.Entry(cuponModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return Ok($"El cupon con Id_Cupon '{id_Cupon}' se dio de baja exitosamente.");
            }
            catch(Exception ex)
            {
                return BadRequest($"Ocurrió un error al intentar dar de baja el cupon con Id_Cupon '{id_Cupon}': {ex.Message}");
            }
        }

        private bool CuponModelExists(int id)
        {
            return _context.Cupones.Any(e => e.Id_Cupon == id);
        }
    }
}

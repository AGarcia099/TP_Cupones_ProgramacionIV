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
                    .Where(c => c.Activo)
                    .ToListAsync();

                if(cupones == null || !cupones.Any())
                {
                    Log.Error("No hay registros de Cupones activos");
                    return NotFound("No hay cupones activos");
                }

                Log.Information("Se llamo al endpoint GetCupones");
                return Ok(cupones);
            }
            catch(Exception ex)
            {
                Log.Error($"Hubo un problema en GetCupones. Error: {ex.Message}");
                return BadRequest($"Ocurrió un error al obtener los cupones: {ex.Message}");
            }
        }

        // GET: api/Cupones/Cliente/{codCliente}
        [HttpGet("Cliente/{codCliente}")]
        public async Task<ActionResult<IEnumerable<CuponModel>>> GetCuponesPorCliente(string codCliente)
        {
            try
            {
                // Obtener todos los cupones del cliente sin las restricciones adicionales
                var cupones = await _context.Cupones_Clientes
                    .Where(cc => cc.CodCliente == codCliente)
                    .Include(cc => cc.Cupon) // Incluir la entidad Cupon asociada
                    .ToListAsync();

                if (cupones == null || !cupones.Any())
                {
                    Log.Error($"No se encontraron cupones asociados con el código de cliente '{codCliente}'");
                    return NotFound($"No se encontraron cupones asociados con el código de cliente '{codCliente}'.");
                }

                Log.Information("Se llamó al endpoint GetCuponesPorCliente");
                return Ok(cupones.Select(cc => cc.Cupon)); // Retornar solo los cupones
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema al obtener los cupones para el cliente con código '{codCliente}': {ex.Message}");
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
                    .FirstOrDefaultAsync(c => c.Id_Cupon == id_Cupon && c.Activo);

                if (cuponModel == null)
                {
                    Log.Error($"No hay cupon activo con el Id_Cupon '{id_Cupon}'");
                    return NotFound($"No existe un cupon activo con el Id_Cupon '{id_Cupon}'.");
                }

                Log.Information("Se llamo al endpoint GetCupon por Id_Cupon");
                return Ok(cuponModel);
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema en GetCupon por Id_Cupon. Error: {ex.Message}");
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
                Log.Error("El id ingresado no coincide con ningun Cupon");
                return BadRequest($"El ID {id_Cupon} no coincide");
            }

            if (!CuponModelExists(id_Cupon))
            {
                Log.Error($"El id '{id_Cupon}' no existe");
                return NotFound($"El cupon con Id_Cupon '{id_Cupon}' no existe");
            }

            _context.Entry(cuponModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint PutCupon");
                return Ok($"El cupon con Id_Cupon '{id_Cupon}' ha sido actualizado exitosamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema en PutCupon. Error: {ex.Message}");
                return BadRequest($"Ocurrió un error al intentar actualizar el cupon con Id_Cupon '{id_Cupon}': {ex.Message}");
            }
        }

        // POST: api/Cupones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CuponModel>> PostCuponModel(CuponModel cuponModel)
        {
            if (string.IsNullOrEmpty(cuponModel.Nombre))
            {
                Log.Error("Se requiere el nombre del cupon");
                return BadRequest("El nombre del cupon es obligatorio");
            }

            if (cuponModel.FechaInicio >= cuponModel.FechaFin)
            {
                Log.Error("La fecha de inicio tiene que ser anterior a la fecha de finalizacion");
                return BadRequest("La fecha de inicio debe ser anterior a la fecha de finalización.");
            }

            try
            {
                _context.Cupones.Add(cuponModel);
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al enpoint PostCupon");
                return Ok($"El cupon '{cuponModel.Id_Cupon}' fue creado exitosamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema en PostCupon. Error: {ex.Message}");
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
                {
                    Log.Error($"No existe el cupon con Id_Cupon '{id_Cupon}'");
                    return NotFound($"El cupon con Id_Cupon '{id_Cupon}' no existe.");
                }

                if (!cuponModel.Activo)
                {
                    Log.Error("El cupon que quieres dar de baja ya esta inactivo");
                    return BadRequest("El cupon ya se encuentra inactivo.");
                }

                cuponModel.Activo = false;
                _context.Entry(cuponModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                Log.Information("Se llamo al enpoint DeleteCupon");
                return Ok($"El cupon con Id_Cupon '{id_Cupon}' se dio de baja exitosamente.");
            }
            catch(Exception ex)
            {
                Log.Error($"Hubo un problema en DeleteCupon. Error: {ex.Message}");
                return BadRequest($"Ocurrió un error al intentar dar de baja el cupon con Id_Cupon '{id_Cupon}': {ex.Message}");
            }
        }

        private bool CuponModelExists(int id)
        {
            return _context.Cupones.Any(e => e.Id_Cupon == id);
        }
    }
}

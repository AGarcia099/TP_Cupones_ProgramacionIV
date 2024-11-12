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
    public class Cupon_CategoriaController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public Cupon_CategoriaController(DataBaseContext context)
        {
            _context = context;
        }

        // GET: api/Cupon_Categoria
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cupon_CategoriaModel>>> GetCupones_Categorias()
        {
            try
            {
                var cuponesCategorias = await _context.Cupones_Categorias.ToListAsync();

                if(cuponesCategorias.Count == 0)
                    return NotFound("No existe ningun Cupon_Categoria.");

                return Ok(cuponesCategorias);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un problema al obtener los Cupones_Categorias. Error: {ex.Message}");
            }
        }

        // GET: api/Cupon_Categoria/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cupon_CategoriaModel>> GetCupon_CategoriaModel(int id)
        {
            try
            {
                var cupon_CategoriaModel = await _context.Cupones_Categorias.FindAsync(id);

                if (cupon_CategoriaModel == null)
                    return NotFound($"El id {id} no existe.");

                return Ok(cupon_CategoriaModel);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un problema al obtener el Cupon_Categoria con el id {id}. Error: {ex.Message}");
            }
        }

        // PUT: api/Cupon_Categoria/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCupon_CategoriaModel(int id, Cupon_CategoriaModel cupon_CategoriaModel)
        {
            if (id != cupon_CategoriaModel.Id_Cupones_Categorias)
                return BadRequest("El ID proporcionado no coincide con el ID del modelo.");

            _context.Entry(cupon_CategoriaModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                return Ok($"El cupon_categoria con id {id} fue modificado correctamente.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Cupon_CategoriaModelExists(id))
                    return NotFound($"El id {id} no existe.");

                else
                {
                    throw;
                }
            }
        }

        // POST: api/Cupon_Categoria
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cupon_CategoriaModel>> PostCupon_CategoriaModel(Cupon_CategoriaModel cupon_CategoriaModel)
        {
            try
            {
                _context.Cupones_Categorias.Add(cupon_CategoriaModel);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCupon_CategoriaModel", new { id = cupon_CategoriaModel.Id_Cupones_Categorias }, cupon_CategoriaModel);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un problema al crear el cupon_categoria. Error: {ex.Message}");
            }
        }

        // DELETE: api/Cupon_Categoria/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCupon_CategoriaModel(int id)
        {
            try
            {
                var cupon_CategoriaModel = await _context.Cupones_Categorias.FindAsync(id);
                
                if (cupon_CategoriaModel == null)
                    return NotFound($"El id {id} no existe.");

                _context.Cupones_Categorias.Remove(cupon_CategoriaModel);
                await _context.SaveChangesAsync();

                return Ok($"El cupon_categoria con id {id} fue borrado exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un problema al eliminar el cupon_categoria. Error: {ex.Message}");
            }
        }

        private bool Cupon_CategoriaModelExists(int id)
        {
            return _context.Cupones_Categorias.Any(e => e.Id_Cupones_Categorias == id);
        }
    }
}

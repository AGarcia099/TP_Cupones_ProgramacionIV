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
    public class CategoriasController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public CategoriasController(DataBaseContext context)
        {
            _context = context;
        }

        // GET: api/Categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaModel>>> GetCategorias()
        {
            try
            {
                var categorias = await _context
                    .Categorias
                    .Include(c => c.Cupones_Categorias)!
                        .ThenInclude(cc => cc.Cupon)
                    .ToListAsync();

                if (categorias.Count == 0)
                    return NotFound("No existe ninguna categoría.");

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un problema al obtener las categorías. Error: {ex.Message}");
            }
        }

        // GET: api/Categorias/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<CategoriaModel>> GetCategoriaModel(int id)
        //{
        //    try
        //    {
        //        var categoriaModel = await _context.Categorias.FindAsync(id);

        //        if (categoriaModel == null)
        //        {
        //            return NotFound($"No existe ninguna categoria con el id {id}");
        //        }

        //        return categoriaModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Hubo un problema. Error: {ex.Message}");
        //    }
        //}

        // PUT: api/Categorias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoriaModel(int id, CategoriaModel categoriaModel)
        {
            if (id != categoriaModel.Id_Categoria)
            {
                return BadRequest("El ID proporcionado no coincide con el de la categoría que intenta actualizar.");
            }

            _context.Entry(categoriaModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                return Ok($"Los datos de la categoria con id {id} fueron modificados correctamente");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaModelExists(id))
                {
                    return NotFound($"No existe una categoria con el id {id}");
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Categorias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CategoriaModel>> PostCategoriaModel(CategoriaModel categoriaModel)
        {
            if (string.IsNullOrEmpty(categoriaModel.Nombre))
                return BadRequest("El nombre de la categoría es obligatorio.");

            try
            {
                var categoriaExistente = await _context.Categorias
                                                .FirstOrDefaultAsync(c => c.Nombre == categoriaModel.Nombre);

                if (categoriaExistente != null)
                {
                    return BadRequest($"La categoria con el nombre '{categoriaModel.Nombre}' ya existe.");
                }

                _context.Categorias.Add(categoriaModel);
                await _context.SaveChangesAsync();

                return Ok($"La categoria '{categoriaModel.Nombre}' fue creada con exito");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un problema al crear la categoria. Error: {ex.Message}");
            }
        }

        // DELETE: api/Categorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoriaModel(int id)
        {
            try
            {
                var categoriaModel = await _context.Categorias.FindAsync(id);
                if (categoriaModel == null)
                {
                    return NotFound($"No existe una categoria con el id {id}.");
                }

                _context.Categorias.Remove(categoriaModel);
                await _context.SaveChangesAsync();

                return Ok($"La categoria con id {id} fue borrada exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un problema al eliminar la categoria. Error: {ex.Message}");
            }
        }

        private bool CategoriaModelExists(int id)
        {
            return _context.Categorias.Any(e => e.Id_Categoria == id);
        }
    }
}

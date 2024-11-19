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
                var categorias = await _context.Categorias.ToListAsync();

                if (categorias.Count == 0)
                {
                    Log.Error("No hay categorias");
                    return NotFound("No existe ninguna categoría.");
                }

                Log.Information("Se llamo al endpoint GetCategorias");
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                Log.Error($"Hub un problema en el endpoint GetCategorias. Error: {ex.Message}");
                return BadRequest($"Hubo un problema al obtener las categorías. Error: {ex.Message}");
            }
        }

        // PUT: api/Categorias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoriaModel(int id, CategoriaModel categoriaModel)
        {
            if (id != categoriaModel.Id_Categoria)
            {
                Log.Error("El id ingresado no existe");
                return BadRequest("El ID proporcionado no coincide con el de la categoría que intenta actualizar.");
            }

            _context.Entry(categoriaModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint PutCategoria");
                return Ok($"Los datos de la categoria con id {id} fueron modificados correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema en PutCategoria. Error {ex.Message}");
                return BadRequest($"Hubo un problema. Error: {ex.Message}");
            }
        }

        // POST: api/Categorias
        [HttpPost]
        public async Task<ActionResult<CategoriaModel>> PostCategoriaModel(CategoriaModel categoriaModel)
        {
            if (string.IsNullOrEmpty(categoriaModel.Nombre))
            {
                Log.Error("Debe ingresar el nombre de la categoria");
                return BadRequest("El nombre de la categoría es obligatorio.");
            }

            try
            {
                var categoriaExistente = await _context.Categorias
                                                .FirstOrDefaultAsync(c => c.Nombre == categoriaModel.Nombre);

                if (categoriaExistente != null)
                {
                    Log.Error("No puede ingresar un nombre de categoria existente");
                    return BadRequest($"La categoria con el nombre '{categoriaModel.Nombre}' ya existe.");
                }

                _context.Categorias.Add(categoriaModel);
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint PostCategoria");
                return Ok($"La categoria '{categoriaModel.Nombre}' fue creada con exito");
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema en PostCategoria. Error: {ex.Message}");
                return BadRequest($"Hubo un problema al crear la categoria. Error: {ex.Message}");
            }
        }

        // DELETE: api/Categorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoriaModel(int id)
        {
            try
            {
                var categoriaModel = await _context.Categorias
                    .Include(c => c.Articulos)
                    .FirstOrDefaultAsync(c => c.Id_Categoria == id);

                if (categoriaModel == null)
                {
                    Log.Error("El id de categoria ingresado no existe");
                    return NotFound($"No existe una categoria con el id {id}.");
                }

                if (categoriaModel.Articulos != null && categoriaModel.Articulos.Any())
                {
                    foreach (var articulo in categoriaModel.Articulos)
                    {
                        articulo.id_categoria = null;
                    }
                }


                _context.Categorias.Remove(categoriaModel);
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint DeleteCategoria");
                return Ok($"La categoria con id {id} fue borrada exitosamente.");
            }
            catch (Exception ex)
            {

                Log.Error($"Hubo un problema en DeleteCategoria. Error: {ex.Message}");
                return BadRequest($"Hubo un problema al eliminar la categoria. Error: {ex.Message}");
            }
        }
    }
}

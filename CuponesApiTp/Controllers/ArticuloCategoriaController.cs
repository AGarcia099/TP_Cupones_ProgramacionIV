using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CuponesApiTp.Data;
using CuponesApiTp.Models;

namespace CuponesApiTp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticuloCategoriaController : Controller
    {
        private readonly DataBaseContext _context;

        public ArticuloCategoriaController(DataBaseContext context)
        {
            _context = context;
        }

        
        [HttpPost("AddArticulosToCategoria")]
        public async Task<ActionResult> AddArticulosToCategoria(int idCategoria, [FromBody] List<int> idsArticulos)
        {
            try
            {
                var categoria = await _context.Categorias
                    .FirstOrDefaultAsync(c => c.Id_Categoria == idCategoria);

                if (categoria == null)
                {
                    return NotFound("La categoría no existe.");
                }

               
                var articulos = await _context.Articulos
                    .Where(a => idsArticulos.Contains(a.Id_Articulo) && a.Activo)
                    .ToListAsync();

                if (!articulos.Any())
                {
                    return NotFound("No se encontraron artículos válidos para asignar.");
                }

                foreach (var articulo in articulos)
                {
                    articulo.id_categoria = idCategoria;
                }

                await _context.SaveChangesAsync();

                return Created("AddArticulosToCategoria", new
                {
                    message = $"Se asignaron {articulos.Count} artículos a la categoría '{categoria.Nombre}'.",
                    articulos = articulos.Select(a => new { a.Id_Articulo, a.Nombre_Articulo })
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al asignar los artículos a la categoría. Detalles: {ex.Message}");
            }
        }
    }
}

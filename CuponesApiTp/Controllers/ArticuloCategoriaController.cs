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
        [HttpPost]
        public async Task<ActionResult> AddCuponesToCategoria(int idCategoria, [FromBody] List<int> idsCupones)
        {
            try
            {
                // Validar si la categoría existe
                var categoria = await _context.Categorias
                    .FirstOrDefaultAsync(c => c.Id_Categoria == idCategoria);

                if (categoria == null)
                {
                    return NotFound("La categoría no existe.");
                }

                // Filtrar los cupones que existen
                var cupones = await _context.Cupones
                    .Where(c => idsCupones.Contains(c.Id_Cupon))
                    .ToListAsync();

                if (!cupones.Any())
                {
                    return NotFound("No se encontraron cupones válidos para asignar.");
                }

                // Crear las relaciones entre los cupones y la categoría
                var relaciones = cupones.Select(c => new Cupon_CategoriaModel
                {
                    Id_Cupon = c.Id_Cupon,
                    Id_Categoria = idCategoria
                }).ToList();

                _context.Cupones_Categorias.AddRange(relaciones);
                await _context.SaveChangesAsync();

                return Created("AddCuponesToCategoria", new
                {
                    message = $"Se asignaron {relaciones.Count} cupones a la categoría '{categoria.Nombre}'.",
                    detalles = relaciones
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al asignar los cupones a la categoría. Detalles: {ex.Message}");
            }
        }


    }
}

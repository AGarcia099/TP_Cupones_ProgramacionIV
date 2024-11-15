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
        public async Task<ActionResult> AddArticuloToCategoria(int idArticulo, int idCategoria)
        {
            try
            {
                // Buscar el artículo por su Id
                var articulo = await _context.Articulos
                    .FirstOrDefaultAsync(a => a.Id_Articulo == idArticulo);

                if (articulo == null)
                {
                    return NotFound("El artículo no existe o no está activo.");
                }

                // Validar si la categoría existe
                var categoria = await _context.Categorias
                    .FirstOrDefaultAsync(c => c.Id_Categoria == idCategoria);

                if (categoria == null)
                {
                    return NotFound("La categoría no existe.");
                }

                // Crear la relación entre artículo y categoría
                var cuponCategoria = new Cupon_CategoriaModel
                {
                    Id_Cupon = articulo.Id_Articulo,
                    Id_Categoria = idCategoria,
                };

                _context.Cupones_Categorias.Add(cuponCategoria);
                await _context.SaveChangesAsync();

                return Created("AddArticuloToCategoria", new
                {
                    message = $"El artículo '{articulo.Nombre_Articulo}' fue agregado exitosamente a la categoría '{categoria.Nombre}'."
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al asignar el artículo a la categoría. Detalles: {ex.Message}");
            }
        }



    }
}

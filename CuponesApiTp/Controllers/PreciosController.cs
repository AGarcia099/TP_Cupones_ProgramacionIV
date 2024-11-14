using CuponesApiTp.Data;
using CuponesApiTp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuponesApiTp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreciosController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public PreciosController(DataBaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticuloModel>>> GetArticulosConPrecios()
        {
            try
            {
                var articulosConPrecios = await _context.Articulos
                    .Include(a => a.Precio) 
                    .Where(a => a.Activo)    
                    .ToListAsync();

                if (articulosConPrecios == null || !articulosConPrecios.Any())
                    return NotFound("No hay artículos disponibles con precios.");

                return Ok(articulosConPrecios);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al obtener los artículos con sus precios: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PrecioModel>> AsignarPrecio(PrecioModel precioModel)
        {
            try
            {
                var articulo = await _context.Articulos.FindAsync(precioModel.Id_Articulo);
                if (articulo == null)
                    return NotFound($"El artículo con ID {precioModel.Id_Articulo} no existe.");

                precioModel.Articulo = articulo; 

                _context.Precios.Add(precioModel);
                await _context.SaveChangesAsync();

                return Ok($"Se asignó un nuevo precio de {precioModel.Precio} al artículo con ID {precioModel.Id_Articulo}.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al intentar asignar el precio: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ModificarPrecio(int id, PrecioModel precioModel)
        {
            try
            {
                // Buscar el precio existente por Id_Precio
                var precioExistente = await _context.Precios.FindAsync(id);
                if (precioExistente == null)
                    return NotFound($"No se encontró un precio con el ID {id}.");

                // Actualizar el valor del precio con el valor proporcionado en precioModel
                precioExistente.Precio = precioModel.Precio;

                // Guardar cambios en la base de datos
                _context.Precios.Update(precioExistente);
                await _context.SaveChangesAsync();

                return Ok($"El precio con ID {id} ha sido actualizado a {precioModel.Precio}.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al intentar modificar el precio: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrecio(int id)
        {
            try
            {
                var precioExistente = await _context.Precios
                    .Include(p => p.Articulo)
                    .FirstOrDefaultAsync(p => p.Id_Precio == id);

                if (precioExistente == null)
                    return NotFound($"No se encontró un precio con el ID {id}.");

                if (precioExistente.Articulo != null)
                {
                    precioExistente.Articulo.Precio = null; 
                    _context.Articulos.Update(precioExistente.Articulo);
                }

                _context.Precios.Remove(precioExistente);
                await _context.SaveChangesAsync();

                return Ok($"El precio con ID {id} fue eliminado y el precio del artículo se estableció en 0.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un problema al intentar eliminar el precio. Error: {ex.Message}");
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CuponesApiTp.Data;
using CuponesApiTp.Models;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace CuponesApiTp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticulosController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public ArticulosController(DataBaseContext context)
        {
            _context = context;
        }

        // GET: api/Articulos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticuloModel>>> GetArticulos()
        {
            try
            {
                var articulos = await _context.Articulos.Include(a => a.Precio).ToListAsync();

                if (articulos.Count == 0)
                {
                    Log.Error("No existen Articulos");
                    return NotFound("No hay articulos");
                }

                Log.Information("Se llamo al endpoint GetArticulos");
                return Ok(articulos);
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema en GetArticulos. error: {ex.Message}");
                return BadRequest($"Hubo un problema. Error: {ex.Message}");
            }
        }

        // GET: api/Articulos/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<ArticuloModel>> GetArticuloModel(int id)
        //{
        //    try
        //    {
        //        var articuloModel = await _context.Articulos.FindAsync(id);

        //        if (articuloModel == null)
        //        {
        //            return NotFound($"No existe un articulo con el id {id}.");
        //        }

        //        return Ok(articuloModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Hubo un problema. Error: {ex.Message}");
        //    }
        //}

        // PUT: api/Articulos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticuloModel(int id, ArticuloModel articuloModel)
        {
            if (id != articuloModel.Id_Articulo)
            {
                Log.Error($"No existe el articulo con id '{id}' ");
                return BadRequest("El ID proporcionado no coincide con el ID del artículo.");
            }

            _context.Entry(articuloModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint de PutArticulo");
                return Ok("Se modificaron los datos del articulo correctamente.");
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema en PutArticulo. Error: {ex.Message}");
                return BadRequest($"Hubo un problema. Error: {ex.Message}");
            }
        }

        // POST: api/Articulos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ArticuloModel>> PostArticuloModel(ArticuloModel articuloModel)
        {
            try
            {

                if (string.IsNullOrEmpty(articuloModel.Nombre_Articulo))
                {
                    Log.Error("El nombre del articulo es necesario");
                    return BadRequest("El nombre del artículo es obligatorio.");
                }

                if (string.IsNullOrEmpty(articuloModel.Descripcion_Articulo))
                {
                    Log.Error("La descripcion del articulo es necesaria");
                    return BadRequest("La descripción del artículo es obligatoria.");
                }

                var articuloExistente = await _context.Articulos
                    .FirstOrDefaultAsync(a => a.Nombre_Articulo == articuloModel.Nombre_Articulo);
                if (articuloExistente != null)
                {
                    Log.Error("Nombre de articulo repetido");
                    return BadRequest($"Ya existe un artículo con el nombre '{articuloModel.Nombre_Articulo}'.");
                }

                var categoriaExistente = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id_Categoria == articuloModel.id_categoria);

                if (categoriaExistente == null)
                {
                    Log.Error("La categoría especificada no existe");
                    return NotFound($"La categoría con ID '{articuloModel.id_categoria}' no existe.");
                }

                _context.Articulos.Add(articuloModel);
                await _context.SaveChangesAsync();

                PrecioModel nuevoPrecio = new PrecioModel
                {
                    Id_Articulo = articuloModel.Id_Articulo,
                    Precio = 0
                };

                _context.Precios.Add(nuevoPrecio);
                await _context.SaveChangesAsync();

                Log.Information("Se llamo al endpoint POST de Articulos");
                return Ok($"El artículo '{articuloModel.Nombre_Articulo}' fue creado exitosamente.");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return BadRequest($"Error interno: {ex.InnerException.Message}");
                }
                return BadRequest($"Ocurrió un error: {ex.Message}");

                Log.Error($"Hubo un problema en PostArticulo. Error: {ex.Message}");
                return BadRequest($"Hubo un problema. Error: {ex.Message}");
            }
        }

        // DELETE: api/Articulos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticuloModel(int id)
        {
            try
            {
                var articuloModel = await _context.Articulos.FindAsync(id);

                if (articuloModel == null)
                {
                    Log.Error($"El articulo con ID {id} no existe");
                    return NotFound($"No existe ningún artículo con el ID {id}.");
                }

                if (!articuloModel.Activo)
                {
                    Log.Error("El articulo ya se encuentra inactivo");
                    return BadRequest("El artículo ya se encuentra inactivo.");
                }

                articuloModel.Activo = false; 
                _context.Entry(articuloModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                Log.Information("El articulo fue dado de baja");
                return Ok($"El artículo con ID {id} fue dado de baja correctamente.");
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema en el endpoint DeleteArticulo. Error: {ex.Message}");
                return BadRequest($"Hubo un problema al intentar dar de baja el artículo. Error: {ex.Message}");
            }
        }
    }
}

using KingCold.Domain.Model;
using KingCold.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KingCold.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : Controller
    {

        private readonly KingColdDbContext _context;

        public CategoriaController(KingColdDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            var categorias = await _context.Categoria.ToListAsync();
            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoria(int id)
        {
            var categoria = await _context.Categoria.FindAsync(id);

            if (categoria == null)
                return NotFound();
            return Ok(categoria);
        }

        [HttpPost("CrearCategoria")]
        public async Task<IActionResult> CrearCategoria([FromBody] Categoria categoria)
        {
            _context.Categoria.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoria);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCategoria(int id, [FromBody] Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return BadRequest();
            }
            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var categoria = await _context.Categoria.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            categoria.Activo = false;

            //_context.Producto.Update(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
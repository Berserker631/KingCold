using KingCold.Domain.Model;
using KingCold.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace KingCold.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedorController : Controller
    {

        private readonly KingColdDbContext _context;

        public ProveedorController(KingColdDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProveedor()
        {
            var proveedores = await _context.Proveedor.ToListAsync();
            return Ok(proveedores);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetProveedores(int id)
        {
            var proveedor = await _context.Proveedor.FindAsync(id);
            return Ok(proveedor);
        }

        [HttpPost("CrearProveedor")]
        public async Task<IActionResult> CrearProveedor([FromBody] Proveedor proveedor)
        {
            _context.Proveedor.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.Id }, proveedor);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> ActualizarProveedor(int id, [FromBody] Proveedor proveedor)
        {
            if (id != proveedor.Id)
            {
                return BadRequest();
            }

            _context.Entry(proveedor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProveedor(int id)
        {
            var proveedor = await _context.Proveedor.FindAsync(id);

            if (id == null)
            {
                return BadRequest();
            }

            proveedor.Activo = false;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

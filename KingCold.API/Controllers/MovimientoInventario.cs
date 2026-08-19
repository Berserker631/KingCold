using KingCold.Domain.Model;
using KingCold.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KingCold.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientoInventarioController : ControllerBase
    {
        private readonly KingColdDbContext _context;

        public MovimientoInventarioController(KingColdDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovimiento(int id)
        {
            var movimiento = await _context.MovimientoInventario.FindAsync(id);

            if (movimiento == null)
                return NotFound();

            return Ok(movimiento);
        }


        [HttpPost]
        public async Task<IActionResult> GenerarMovimiento([FromBody] MovimientoInventario movimientoInventario)
        {
            var producto = await _context.Producto.FindAsync(movimientoInventario.ProductoId);

            if (producto == null)
            {
                return NotFound();
            }

            _context.MovimientoInventario.Add(movimientoInventario);

            if (movimientoInventario.TipoMovimientoId == 1)
            {
                producto.Stock += movimientoInventario.Cantidad;
            }
            else if (movimientoInventario.TipoMovimientoId == 2)
            {
                producto.Stock -= movimientoInventario.Cantidad;
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovimiento), new { movimientoInventario.id }, movimientoInventario);
        }
    }
}

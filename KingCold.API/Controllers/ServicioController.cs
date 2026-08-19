using KingCold.Domain.Model;
using KingCold.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KingCold.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicioController : ControllerBase
{
    private readonly KingColdDbContext _context;

    public ServicioController(KingColdDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Servicio>>> GetServicios()
    {
        return Ok(await _context.Servicio
            .AsNoTracking()
            .OrderBy(servicio => servicio.Nombre)
            .ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Servicio>> GetServicio(int id)
    {
        var servicio = await _context.Servicio
            .AsNoTracking()
            .FirstOrDefaultAsync(servicio => servicio.Id == id);

        return servicio is null ? NotFound() : Ok(servicio);
    }

    [HttpPost]
    public async Task<ActionResult<Servicio>> CrearServicio([FromBody] Servicio servicio)
    {
        var error = await ValidarServicio(servicio);
        if (error is not null)
            return BadRequest(error);

        servicio.Nombre = servicio.Nombre.Trim();
        servicio.Descripcion = NormalizarDescripcion(servicio.Descripcion);
        _context.Servicio.Add(servicio);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetServicio), new { id = servicio.Id }, servicio);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> ActualizarServicio(int id, [FromBody] Servicio datosServicio)
    {
        if (id != datosServicio.Id)
            return BadRequest("El identificador de la URL no coincide con el del servicio.");

        var error = await ValidarServicio(datosServicio);
        if (error is not null)
            return BadRequest(error);

        var servicio = await _context.Servicio.FindAsync(id);
        if (servicio is null)
            return NotFound();

        servicio.Nombre = datosServicio.Nombre.Trim();
        servicio.CategoriaId = datosServicio.CategoriaId;
        servicio.PrecioBase = datosServicio.PrecioBase;
        servicio.Descripcion = NormalizarDescripcion(datosServicio.Descripcion);
        servicio.Activo = datosServicio.Activo;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> EliminarServicio(int id)
    {
        var servicio = await _context.Servicio.FindAsync(id);
        if (servicio is null)
            return NotFound();

        servicio.Activo = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<string?> ValidarServicio(Servicio servicio)
    {
        if (string.IsNullOrWhiteSpace(servicio.Nombre))
            return "El nombre del servicio es obligatorio.";

        if (servicio.Nombre.Trim().Length > 150)
            return "El nombre no puede tener más de 150 caracteres.";

        if (servicio.PrecioBase < 0)
            return "El precio base no puede ser negativo.";

        if (servicio.Descripcion?.Length > 255)
            return "La descripción no puede tener más de 255 caracteres.";

        var categoriaExiste = await _context.Categoria
            .AsNoTracking()
            .AnyAsync(categoria => categoria.Id == servicio.CategoriaId && categoria.Activo);

        return categoriaExiste ? null : "La categoría indicada no existe o está inactiva.";
    }

    private static string? NormalizarDescripcion(string? descripcion) =>
        string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
}

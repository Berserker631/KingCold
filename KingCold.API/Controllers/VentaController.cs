using KingCold.Domain.Model;
using KingCold.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KingCold.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentaController : ControllerBase
{
    private readonly KingColdDbContext _context;

    public VentaController(KingColdDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetVentas()
    {
        var ventas = await _context.Venta
            .AsNoTracking()
            .Include(venta => venta.Detalles)
            .OrderByDescending(venta => venta.Fecha)
            .ToListAsync();

        return Ok(ventas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVenta(int id)
    {
        var venta = await _context.Venta
            .AsNoTracking()
            .Include(venta => venta.Detalles)
            .FirstOrDefaultAsync(venta => venta.Id == id);

        return venta is null ? NotFound() : Ok(venta);
    }

    [HttpPost]
    public async Task<IActionResult> CrearVenta([FromBody] CrearVentaRequest request)
    {
        if (request.Detalles is null || request.Detalles.Count == 0)
            return BadRequest("La venta debe incluir al menos un detalle.");

        if (request.Detalles.Any(detalle => detalle.Cantidad <= 0 || detalle.ItemId <= 0))
            return BadRequest("Cada detalle debe tener un ítem válido y una cantidad mayor que cero.");

        if (request.ClienteId == 0 || !await _context.Cliente.AnyAsync(cliente => cliente.Id == request.ClienteId && cliente.Activo))
            return BadRequest("El cliente indicado no existe o está inactivo.");

        if (request.EmpleadoId == 0 || !await _context.Empleado.AnyAsync(empleado => empleado.Id == request.EmpleadoId && empleado.Activo))
            return BadRequest("El empleado indicado no existe o está inactivo.");

        if (request.Detalles.Any(detalle => !EsTipoItemValido(detalle.TipoItem)))
            return BadRequest("El tipo de ítem debe ser 'Producto' o 'Servicio'.");

        var cantidadesPorProducto = request.Detalles
            .Where(detalle => EsProducto(detalle.TipoItem))
            .GroupBy(detalle => detalle.ItemId)
            .ToDictionary(grupo => grupo.Key, grupo => grupo.Sum(detalle => detalle.Cantidad));
        var productos = await _context.Producto
            .Where(producto => cantidadesPorProducto.Keys.Contains(producto.Id) && producto.Activo)
            .ToDictionaryAsync(producto => producto.Id);

        if (productos.Count != cantidadesPorProducto.Count)
            return BadRequest("Uno o más productos no existen o están inactivos.");

        var idsServicios = request.Detalles
            .Where(detalle => EsServicio(detalle.TipoItem))
            .Select(detalle => detalle.ItemId)
            .Distinct()
            .ToList();
        var serviciosActivos = await _context.Servicio
            .Where(servicio => idsServicios.Contains(servicio.Id) && servicio.Activo)
            .ToDictionaryAsync(servicio => servicio.Id);
        if (serviciosActivos.Count != idsServicios.Count)
            return BadRequest("Uno o más servicios no existen o están inactivos.");

        var sinStock = cantidadesPorProducto.FirstOrDefault(item => productos[item.Key].Stock < item.Value);
        if (!sinStock.Equals(default(KeyValuePair<int, int>)))
            return BadRequest($"Stock insuficiente para el producto con id {sinStock.Key}.");

        await using var transaction = await _context.Database.BeginTransactionAsync();
        var venta = new Venta
        {
            ClienteId = request.ClienteId,
            EmpleadoId = request.EmpleadoId,
            Fecha = DateTime.UtcNow
        };

        foreach (var item in request.Detalles)
        {
            var esProducto = EsProducto(item.TipoItem);
            var precioUnitario = esProducto
                ? productos[item.ItemId].PrecioVenta
                : serviciosActivos[item.ItemId].PrecioBase;
            var subtotal = precioUnitario * item.Cantidad;
            venta.Detalles.Add(new DetalleVenta
            {
                TipoItem = item.TipoItem.Trim(),
                ItemId = item.ItemId,
                Cantidad = item.Cantidad,
                PrecioUnitario = precioUnitario,
                Subtotal = subtotal
            });
            if (esProducto)
                productos[item.ItemId].Stock -= item.Cantidad;
        }

        venta.Total = venta.Detalles.Sum(detalle => detalle.Subtotal);
        _context.Venta.Add(venta);
        await _context.SaveChangesAsync();

        foreach (var detalle in venta.Detalles.Where(detalle => EsProducto(detalle.TipoItem)))
        {
            _context.MovimientoInventario.Add(new MovimientoInventario
            {
                ProductoId = detalle.ItemId,
                UsuarioId = request.EmpleadoId,
                TipoMovimientoId = 2,
                Cantidad = detalle.Cantidad,
                Fecha = venta.Fecha,
                Observacion = $"Salida por venta #{venta.Id}"
            });
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return CreatedAtAction(nameof(GetVenta), new { id = venta.Id }, venta);
    }

    private static bool EsProducto(string? tipoItem) =>
        string.Equals(tipoItem?.Trim(), "Producto", StringComparison.OrdinalIgnoreCase);

    private static bool EsServicio(string? tipoItem) =>
        string.Equals(tipoItem?.Trim(), "Servicio", StringComparison.OrdinalIgnoreCase);

    private static bool EsTipoItemValido(string? tipoItem) => EsProducto(tipoItem) || EsServicio(tipoItem);
}

public class CrearVentaRequest
{
    public int ClienteId { get; set; }
    public int EmpleadoId { get; set; }
    public List<CrearDetalleVentaRequest> Detalles { get; set; } = [];
}

public class CrearDetalleVentaRequest
{
    public string TipoItem { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public int Cantidad { get; set; }
}

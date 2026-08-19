namespace KingCold.Domain.Model;

public class Venta
{
    public int Id { get; set; }
    public int? ClienteId { get; set; }
    public int? EmpleadoId { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }

    public Cliente? Cliente { get; set; }
    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
}

namespace KingCold.Domain.Model;

public class MovimientoInventario
{
    public int id { get; set; }
    public int ProductoId { get; set; }
    public int UsuarioId { get; set; }
    public int TipoMovimientoId { get; set; }
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; }
    public string? Observacion { get; set; }
}

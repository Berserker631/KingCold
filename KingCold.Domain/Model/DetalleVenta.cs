using System.Text.Json.Serialization;

namespace KingCold.Domain.Model;

public class DetalleVenta
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    public string TipoItem { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

    [JsonIgnore]
    public Venta? Venta { get; set; }
}

namespace KingCold.Domain.Model;

public class Servicio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public decimal PrecioBase { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}

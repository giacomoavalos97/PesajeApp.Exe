namespace PesajeApp.Models;

public class Producto
{
    public int Id { get; set; }

    public int? SedeId { get; set; }

    public int? Cod { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public decimal Precio { get; set; }

    public int Caducidad { get; set; }

    public bool Estado { get; set; }

    public string? SedeNombre { get; set; }

    public int? Cantidad { get; set; }
}
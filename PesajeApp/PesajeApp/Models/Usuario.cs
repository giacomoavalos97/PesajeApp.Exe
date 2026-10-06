namespace PesajeApp.Models;

public class Usuario
{
    public int Id { get; set; }

    public int? SedeId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Clave { get; set; } = string.Empty;

    public string? Rol { get; set; }

    public string Turno { get; set; } = string.Empty;

    public bool Estado { get; set; }

    public string? SedeNombre { get; set; }
}

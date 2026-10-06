namespace PesajeApp.Printing;

public class PesajeReporteData
{
    public int Id { get; set; }

    // Empresa
    public string? EmpresaNombre { get; set; }
    public string? EmpresaTitulo2 { get; set; }
    public string? EmpresaTitulo3 { get; set; }

    // Sede
    public string? SedeNombre { get; set; }

    // Producto
    public string? ProductoCodigo { get; set; }
    public string ProductoDescripcion { get; set; } = string.Empty;
    public int? Cantidad { get; set; }

    // Pesaje
    public decimal Peso { get; set; }
    public decimal PesoTara { get; set; }
    public decimal PesoNeto { get; set; }

    // Precio
    public decimal Precio { get; set; }
    public decimal Importe { get; set; }

    // Datos adicionales
    public string? Lote { get; set; }
    public string? CodAlt { get; set; }
    public string? OP { get; set; }
    public string? Kanban { get; set; }

    // Usuario / turno
    public string? Turno { get; set; }
    public string? Usuario { get; set; }

    // Fecha
    public DateTime? FechaLocal { get; set; }

    // Configuración de impresión
    public bool MostrarPrecio { get; set; }
    public bool MostrarLote { get; set; }
    public bool MostrarCodigoBarras { get; set; }
    public bool MostrarFechaVencimiento { get; set; }
    public bool MostrarFechaPeriodo { get; set; }
    public bool MostrarPesoNeto { get; set; }
    public bool MostrarPesoTara { get; set; }
}
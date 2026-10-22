namespace PesajeApp.Models
{
    public class Pesaje
    {
        public int Id { get; set; }

        public string? CodAlt { get; set; }

        public string? OP { get; set; }

        public string? Kanban { get; set; }

        public int? SedeId { get; set; }

        public int? Cantidad { get; set; }

        public int? ProductoId { get; set; }

        public int? UsuarioId { get; set; }

        public string? Turno { get; set; }

        public DateTime? FechaLocal { get; set; }

        public DateTime? FechaServer { get; set; }

        public decimal? Peso { get; set; }

        public decimal? Precio { get; set; }

        public decimal? Importe { get; set; }

        public bool Estado { get; set; } = true;

        public string? Lote { get; set; }

        public decimal? PesoTara { get; set; }

        public decimal? PesoNeto { get; set; }

        public string? EmpresaNombre { get; set; }

        public string? EmpresaTitulo2 { get; set; }

        public string? EmpresaTitulo3 { get; set; }

        public int? FlagPrecio { get; set; }

        public int? FlagLote { get; set; }

        public int? FlagBar { get; set; }

        public int? FlagFechaVencimiento { get; set; }

        public int? FlagFechaPeriodo { get; set; }

        public int? FlagPesoNeto { get; set; }

        public int? FlagPesoTara { get; set; }

        public string? ProductoCodigo { get; set; }

        public string? ProductoDescripcion { get; set; }

        public string? UsuarioNombre { get; set; }

        public string? SedeNombre { get; set; }
    }
}
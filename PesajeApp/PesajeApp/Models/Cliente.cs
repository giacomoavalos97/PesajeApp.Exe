using System;
using System.Collections.Generic;
using System.Text;

namespace PesajeApp.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public long? Cod { get; set; }
        public string? Nombres { get; set; }
        public int? SedeId { get; set; }
        public bool Estado { get; set; }
        public string? SedeNombre { get; set; }
    }
}

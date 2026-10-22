using System;
using System.Collections.Generic;
using System.Text;

namespace PesajeApp.Models
{
    public class Sede
    {
        public int Id { get; set; }
        public long Cod {  get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }

    }
}

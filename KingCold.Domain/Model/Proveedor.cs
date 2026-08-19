using System;
using System.Collections.Generic;
using System.Text;

namespace KingCold.Domain.Model
{
    public class Proveedor
    {
        public int Id { get; set; }
        
        public string Nombre { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public bool Activo { get; set; }
    }
}

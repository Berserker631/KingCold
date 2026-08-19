using System;
using System.Collections.Generic;
using System.Text;

namespace KingCold.Domain.Model
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public bool Activo { get; set; }
    }
}

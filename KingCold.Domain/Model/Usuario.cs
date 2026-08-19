using System;
using System.Collections.Generic;
using System.Text;

namespace KingCold.Domain.Model
{
    public class Usuario
    {
        public int Id { get; set; }

        public string NombreUsuario { get; set; } = string.Empty;

        public string Contraseña { get; set; } = string.Empty;

        public short? Rol { get; set; }

        public bool Activo { get; set; }
    }
}

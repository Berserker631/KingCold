using KingCold.Domain.Model;
using KingCold.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KingCold.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly KingColdDbContext _context;

        public AuthController(KingColdDbContext context)
        {
            _context = context;
        }    

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(x =>
                    x.NombreUsuario == request.NombreUsuario &&
                    x.Contraseña == request.Contraseña);

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensaje = "Usuario o contraseña incorrectos"
                });
            }

            return Ok(new
            {
                mensaje = "Login correcto",
                usuario = usuario.NombreUsuario,
                rol = usuario.Rol
            });
        }



        public class LoginRequest
        {
            public string NombreUsuario { get; set; } = string.Empty;

            public string Contraseña { get; set; } = string.Empty;
        }
    }
}
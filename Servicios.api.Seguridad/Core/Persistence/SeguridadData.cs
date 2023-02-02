using Microsoft.AspNetCore.Identity;
using Servicios.api.Seguridad.Core.Entities;

namespace Servicios.api.Seguridad.Core.Persistence
{
    public class SeguridadData
    {
        public static async Task InsertarUsuario(SeguridadContexto context, UserManager<Usuario> usuarioManager)
        {
            if (!usuarioManager.Users.Any())
            {
                var usuario = new Usuario()
                {
                    Nombre = "Gustavo",
                    Apellido = "Ortiz",
                    Direccion = "Lago Guija 72",
                    UserName = "jdxtavo",
                    Email = "jdxtavo@gmail.com"
                };
                await usuarioManager.CreateAsync(usuario, "Admin123*");

            }
        }
    }
}

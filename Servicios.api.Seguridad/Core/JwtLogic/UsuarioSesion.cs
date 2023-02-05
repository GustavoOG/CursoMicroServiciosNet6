namespace Servicios.api.Seguridad.Core.JwtLogic
{
    public class UsuarioSesion : IUsuarioSesion
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UsuarioSesion(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string GetUsuarioSession()
        {
            var userName = _contextAccessor.HttpContext.User?.Claims?.FirstOrDefault(m => m.Type == "username")?.Value;

            return userName;
        }
    }
}

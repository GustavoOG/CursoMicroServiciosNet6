using AutoMapper;
using Servicios.api.Seguridad.Core.Entities;

namespace Servicios.api.Seguridad.Core.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Usuario, UsuarioDto>();

        }

    }
}

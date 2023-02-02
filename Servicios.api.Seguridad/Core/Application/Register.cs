using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Servicios.api.Seguridad.Core.Dto;
using Servicios.api.Seguridad.Core.Entities;
using Servicios.api.Seguridad.Core.JwtLogic;
using Servicios.api.Seguridad.Core.Persistence;

namespace Servicios.api.Seguridad.Core.Application
{
    public class Register
    {

        public class UsuarioRegisterCommand : IRequest<UsuarioDto>
        {

            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }

        }

        /// <summary>
        /// Para validar la clase
        /// </summary>
        public class UsuarioRegisterValidation : AbstractValidator<UsuarioRegisterCommand>
        {
            public UsuarioRegisterValidation()
            {
                RuleFor(m => m.Nombre).NotEmpty();
                RuleFor(m => m.Apellido).NotEmpty();
                RuleFor(m => m.Username).NotEmpty();
                RuleFor(m => m.Email).NotEmpty();
                RuleFor(m => m.Password).NotEmpty();

            }
        }

        public class UsuarioRegisterHandler : IRequestHandler<UsuarioRegisterCommand, UsuarioDto>
        {
            private readonly SeguridadContexto _contexto;
            private readonly UserManager<Usuario> _userManager;
            private readonly IMapper _mapper;
            private readonly IJwtGenerator _jwtGenerator;

            public UsuarioRegisterHandler(SeguridadContexto contexto, UserManager<Usuario> userManager, IMapper mapper, IJwtGenerator jwtGenerator)
            {
                _contexto = contexto;
                _userManager = userManager;
                _mapper = mapper;
                _jwtGenerator = jwtGenerator;
            }

            public async Task<UsuarioDto> Handle(UsuarioRegisterCommand request, CancellationToken cancellationToken)
            {
                var Existe = await _contexto.Users.AnyAsync(m => m.Email == request.Email);
                if (Existe)
                {
                    throw new Exception("El email del usuario ya existe en la base de datos");
                }
                Existe = await _contexto.Users.AnyAsync(m => m.UserName == request.Username);
                if (Existe)
                {
                    throw new Exception("El UserName del usuario ya existe en la base de datos");
                }
                var usuario = new Usuario
                {
                    Nombre = request.Nombre,
                    Apellido = request.Apellido,
                    Email = request.Email,
                    UserName = request.Username
                };

                var resultado = await _userManager.CreateAsync(usuario, request.Password);
                if (resultado.Succeeded)
                {
                    var usuarioDTO = _mapper.Map<Usuario, UsuarioDto>(usuario);
                    usuarioDTO.token = _jwtGenerator.CreateToken(usuario);
                    return usuarioDTO;
                }

                throw new Exception("No se pudo registrar el usuario");
            }
        }

    }
}

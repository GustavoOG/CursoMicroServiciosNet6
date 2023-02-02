using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Servicios.api.Seguridad.Core.Dto;
using Servicios.api.Seguridad.Core.Entities;
using Servicios.api.Seguridad.Core.JwtLogic;
using Servicios.api.Seguridad.Core.Persistence;

namespace Servicios.api.Seguridad.Core.Application
{
    public class Login
    {
        public class UsuarioLoginCommand : IRequest<UsuarioDto>
        {

            public string Email { get; set; }

            public string Password { get; set; }
        }

        public class UsuarioLoginValidation : AbstractValidator<UsuarioLoginCommand>
        {

            public UsuarioLoginValidation()
            {
                RuleFor(m => m.Email).NotEmpty();
                RuleFor(m => m.Password).NotEmpty();
            }
        }

        public class UsuarioLoginHandler : IRequestHandler<UsuarioLoginCommand, UsuarioDto>
        {
            private readonly SeguridadContexto _contexto;
            private readonly UserManager<Usuario> _userManager;
            private readonly IMapper _mapper;
            private readonly IJwtGenerator _jwtGenerator;
            private readonly SignInManager<Usuario> _signInManager;

            public UsuarioLoginHandler(SeguridadContexto contexto, UserManager<Usuario> userManager, IMapper mapper, IJwtGenerator jwtGenerator, SignInManager<Usuario> signInManager)
            {
                _contexto = contexto;
                _userManager = userManager;
                _mapper = mapper;
                _jwtGenerator = jwtGenerator;
                _signInManager = signInManager;
            }

            public async Task<UsuarioDto> Handle(UsuarioLoginCommand request, CancellationToken cancellationToken)
            {
                var usuario = await _userManager.FindByEmailAsync(request.Email);
                if (usuario == null)
                {
                    throw new Exception("El usuario no existe");
                }

                var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, request.Password, false);
                if (resultado.Succeeded)
                {
                    var usuarioDTO = _mapper.Map<Usuario, UsuarioDto>(usuario);
                    usuarioDTO.token = _jwtGenerator.CreateToken(usuario);
                    return usuarioDTO;
                }
                throw new Exception("Login incorrecto");
            }
        }
    }
}

using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Servicios.api.Seguridad.Core.Application;
using Servicios.api.Seguridad.Core.Entities;
using Servicios.api.Seguridad.Core.JwtLogic;
using Servicios.api.Seguridad.Core.Persistence;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

//Conexion a base de datos
builder.Services.AddDbContext<SeguridadContexto>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ConexionDB"));
});

//Instnacia de identity Core
var builderUsuario = builder.Services.AddIdentityCore<Usuario>();
//
var identityBuilder = new IdentityBuilder(builderUsuario.UserType, builderUsuario.Services);
identityBuilder.AddEntityFrameworkStores<SeguridadContexto>();
identityBuilder.AddSignInManager<SignInManager<Usuario>>();
builder.Services.TryAddSingleton<ISystemClock, SystemClock>();

builder.Services.AddMediatR(typeof(Register.UsuarioRegisterCommand).Assembly);
builder.Services.AddAutoMapper(typeof(Register.UsuarioRegisterHandler));
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddScoped<IUsuarioSesion, UsuarioSesion>();

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SjdCAnYwalCRTE7EAdgFQh5N0xN0oJvF"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateAudience = false,
        ValidateIssuer = false
    };
});

// Add services to the container.
//depreciado
builder.Services.AddControllers();//.AddFluentValidation(m => m.RegisterValidatorsFromAssemblyContaining<Register>());


builder.Services.AddValidatorsFromAssemblyContaining<Register>(); // register validators
builder.Services.AddFluentValidationAutoValidation(); // the same old MVC pipeline behavior
builder.Services.AddFluentValidationClientsideAdapters(); // for client side



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Habiitamos cors
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsRule", rule =>
    {
        rule.AllowAnyHeader().AllowAnyMethod().WithOrigins("*");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsRule");
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();


using (var contexto = app.Services.CreateScope())
{
    var services = contexto.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<Usuario>>();
        var _contextoEF = services.GetRequiredService<SeguridadContexto>();

        SeguridadData.InsertarUsuario(_contextoEF, userManager).Wait();

    }
    catch (Exception exp)
    {
        var loggin = services.GetRequiredService<ILogger<Program>>();
        loggin.LogError(exp, "error cuando registra usuario");
    }
}


app.Run();

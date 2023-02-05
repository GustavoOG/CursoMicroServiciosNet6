using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;



var builder = WebApplication.CreateBuilder(args);
//IConfiguration configuration = new ConfigurationBuilder()
//                            .AddJsonFile("ocelot.json")
//                            .Build();
//builder.Configuration.AddJsonFile("ocelot.json");


//Token
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
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Ocelot
//builder.Services.AddOcelot(configuration);
builder.Configuration
    .AddJsonFile("ocelot.json");
//.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json");
builder.Services.AddOcelot();



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
app.UseOcelot().Wait();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();

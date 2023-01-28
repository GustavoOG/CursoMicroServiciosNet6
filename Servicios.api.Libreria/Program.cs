using Servicios.api.Libreria.Core;
using Servicios.api.Libreria.Core.ContextMongoDB;
using Servicios.api.Libreria.Repository;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.


//coneccion a MongoDB
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoDB"));

//options =>
//{
//    options.ConnectionString = builder.Configuration.GetSection("MongoDB:ConnectionString").Value;
//    options.Database = builder.Configuration.GetSection("MongoDB:Database").Value;
//}



//genera persistencia
builder.Services.AddSingleton<MongoSettings>();

//Crea nueva instancia en cada invocacion
builder.Services.AddTransient<IAutorContext, AutorContext>();
builder.Services.AddTransient<IAutorRepository, AutorRepository>();

//Solo funciona en una peticion, se destruye inmediatamente con el response
builder.Services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

builder.Services.AddControllers();
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

app.MapControllers();

app.Run();

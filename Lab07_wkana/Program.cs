using Lab07_wkana.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Add services to the container.
builder.Services.AddControllers();

// instalacion swagger 
builder.Services.AddEndpointsApiExplorer();
// codiguracion de swagger para poder pasar bearer en el header de la peticion
builder.Services.AddSwaggerGen();

// ------------------------- app construida -------------------------
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // uso de swagger
    app.UseSwagger();
    // extra de configuracion de swagger para poder pasar bearer en el header de la peticion
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API v1");
        c.RoutePrefix = string.Empty; // Esto hace que Swagger esté en "/"
        c.DefaultModelsExpandDepth(-1); // Ocultar modelos por defecto si no lo necesitas
    });
}

app.UseHttpsRedirection();



app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
// registrar middleware de validacion de parametros
app.UseMiddleware<ParameterValidationMiddleware>();

// -------------------------------- Correr app --------------------------------
app.Run();
using System.Text;
using Lab07_wkana.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Add services to the container.
builder.Services.AddControllers();

// ------------------------- Configuracion de JWT -------------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };
    });

// instalacion swagger 
builder.Services.AddEndpointsApiExplorer();
// codiguracion de swagger para poder pasar bearer en el header de la peticion
builder.Services.AddSwaggerGen(c =>
{
    // Configuración de JWT Bearer en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please enter your JWT token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// ------------------------- app construida -------------------------
var app = builder.Build();

// REGISTRAR PRIMERO el middleware de manejo de errores
app.UseMiddleware<ErrorHandlingMiddleware>(); // Registrar el middleware de manejo de errores

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

// 2. Autenticación y autorización
app.UseAuthentication(); // Primero autenticación parte 3
app.UseAuthorization();  // Luego autorización parte 3


// 3. Middlewares personalizados
//app.UseMiddleware<RoleBasedAccessMiddleware>(); // Registrar el middleware de gestión de roles
app.UseMiddleware<ParameterValidationMiddleware>(); // registrar middleware de validacion de parametros


// Aplicar middleware solo para /api/Auth/admin
app.MapWhen(context => context.Request.Path.StartsWithSegments("/api/Auth/admin"), authApp =>
{
    authApp.UseMiddleware<RoleBasedAccessMiddleware>(); // Aquí aplicas el middleware específico para /api/Auth/admin
    authApp.UseRouting();
    //authApp.UseAuthentication();
    //authApp.UseAuthorization();
    authApp.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
});

// Mapeo de controladores
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Este mapeo debe estar fuera de MapWhen
});

// -------------------------------- Correr app --------------------------------
app.Run();
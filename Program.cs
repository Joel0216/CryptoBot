using CryptoBot.Data;
using CryptoBot.Services.Features.Crypto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de conexión a la base de datos SQL Server (Somee)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("workstation id=CryptoBot.mssql.somee.com;packet size=4096;user id=joel0804_SQLLogin_1;pwd=iua49sfosa;data source=CryptoBot.mssql.somee.com;persist security info=False;initial catalog=CryptoBot;TrustServerCertificate=True")
);

// 2. Servicio de lógica para cifrado
builder.Services.AddScoped<CryptoService>();

// 3. Configuración JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// 4. Agregar controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CryptoBot API",
        Version = "v1",
        Description = "API para encriptación y desencriptación con método César"
    });

    // 🔐 Configurar Bearer token en Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresa tu token JWT como: Bearer tu_token_aqui",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 5. Mostrar Swagger en entorno desarrollo
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CryptoBot API V1");
    c.RoutePrefix = "swagger";
});

}

// Middleware personalizado para bloquear todas las IP excepto 187.155.101.200
app.Use(async (context, next) =>
{
    var ipPermitidas = new[] { "187.155.101.200", "::1", "127.0.0.1" };

    var ipCliente = context.Connection.RemoteIpAddress?.ToString();

    // Evita bloqueo si estás en entorno de desarrollo
    var env = app.Environment;
    if (env.IsDevelopment() || ipPermitidas.Contains(ipCliente))
    {
        await next();
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Acceso denegado: esta API solo está disponible desde una IP autorizada.");
    }
});


// 7. Seguridad
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

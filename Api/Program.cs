using System.Text;
using Application.Abstractions;
using Application.Services;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL; // <- Importante para PostgreSQL

var builder = WebApplication.CreateBuilder(args);

var jwtKey =
    Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException(
        "JWT Secret Key no configurada. "
            + "Configure la variable de entorno JWT_SECRET_KEY o agregue Jwt:Key en appsettings.Development.json"
    );
}

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "AdminPolicy",
        policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("Administrador") || context.User.IsInRole("Admin")
            )
    );

    options.AddPolicy(
        "AdminOrSuperAdminPolicy",
        policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("Administrador")
                || context.User.IsInRole("Admin")
                || context.User.IsInRole("SuperAdministrador")
                || context.User.IsInRole("SuperAdmin")
            )
    );
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

// ---- BEGIN: Geocoding proxy requirements ----
// Cache en memoria (evita golpear Nominatim con la misma query)
builder.Services.AddMemoryCache();

// Registrar HttpClient configurado para Nominatim
builder.Services.AddHttpClient(
    "Nominatim",
    client =>
    {
        client.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
        // User-Agent obligatorio según la política de Nominatim (incluye email)
        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "HighFitApp/1.0 (simisantarelli@gmail.com)"
        );
        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("es");
    }
);

// --

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Ingrese el token JWT en el formato: Bearer {token}",
        }
    );

    c.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

// =========================================================================
// CAMBIO CRÍTICO: CONFIGURACIÓN DE POSTGRESQL PARA RENDER
// =========================================================================

// 1. Obtener la cadena de conexión de appsettings (para desarrollo local)
var connectionString = builder.Configuration.GetConnectionString("PostgreSql");

// 2. Si la cadena es nula o vacía (como en el entorno de Render),
//    intentar obtenerla de la variable de entorno estándar de Render.
if (string.IsNullOrEmpty(connectionString))
{
    // Esta variable es inyectada automáticamente por Render al vincular la DB
    connectionString = Environment.GetEnvironmentVariable("InternalDatabaseURL");
}

// 3. Configurar el DbContext usando la cadena de conexión determinada
//    y el proveedor Npgsql.
builder.Services.AddDbContext<GymDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// =========================================================================
// FIN DEL CAMBIO
// =========================================================================

builder.Services.AddScoped<IAlumnoService, AlumnoService>();
builder.Services.AddScoped<IProfesorService, ProfesorService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IClaseService, ClaseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<IMembresiaService, MembresiaService>();
builder.Services.AddScoped<ISucursalService, SucursalService>();
builder.Services.AddScoped<ISalaService, SalaService>();

builder.Services.AddScoped<IAlumnoRepository, AlumnoRepository>();
builder.Services.AddScoped<IProfesorRepository, ProfesorRepository>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IClaseRepository, ClaseRepository>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IPagoRepository, PagoRepository>();
builder.Services.AddScoped<IMembresiaRepository, MembresiaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ISucursalRepository, SucursalRepository>();
builder.Services.AddScoped<ISalaRepository, SalaRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
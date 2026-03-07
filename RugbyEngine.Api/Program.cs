using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RugbyEngine.Api.Data;
using RugbyEngine.Api.Data.Extensions;
using RugbyEngine.Api.Middleware;
using RugbyEngine.Api.Services;
using Scalar.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ApiDbContext>(opt =>
{
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST")
                ?? builder.Configuration["Database:Host"]
                ?? throw new InvalidOperationException("Database Host no configurado.");
    var dbPort = Environment.GetEnvironmentVariable("DB_PORT")
                ?? builder.Configuration["Database:Port"]
                ?? throw new InvalidOperationException("Database Port no configurado.");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME")
                ?? builder.Configuration["Database:Name"]
                ?? throw new InvalidOperationException("Database Name no configurado.");
    var dbUser = Environment.GetEnvironmentVariable("DB_USER")
                ?? builder.Configuration["Database:User"]
                ?? throw new InvalidOperationException("Database User no configurado.");
    var dbPass = Environment.GetEnvironmentVariable("DB_PASS")
                ?? builder.Configuration["Database:Pass"]
                ?? throw new InvalidOperationException("Database Password no configurado.");
    var connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User Id={dbUser};Password={dbPass};";

    var dbCertificatePath = Environment.GetEnvironmentVariable("DB_CERTIFICATE_PATH")
                        ?? Environment.GetEnvironmentVariable("DB_CARTIFICATE_PATH")
                        ?? builder.Configuration["Database:CertificatePath"];

    if (!string.IsNullOrWhiteSpace(dbCertificatePath))
    {
        connectionString += $"Ssl Mode=Require;Trust Server Certificate=true;Root Certificate={dbCertificatePath};";
    }
    opt.UseLazyLoadingProxies()
       .UseNpgsql(connectionString);

});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RugbyEngine",
        Version = "v1",
        Description = "API con autenticacion JWT",
        Contact = new OpenApiContact
        {
            Name = "RugbyEngine"
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa tu token JWT obtenido del endpoint /api/auth/login\n\nEjemplo: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    // ⬇️ CAMBIO AQUÍ
    options.AddSecurityRequirement(document => new()
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// Configuracion de JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
            ?? jwtSettings["Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer no configurada");
var Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
            ?? jwtSettings["Audience"]
            ?? throw new InvalidOperationException("JWT Audience no configurada");
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
            ?? jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey no configurada");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Issuer,
        ValidAudience = Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddRepositories();

builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var app = builder.Build();

// Test database connection on startup
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

    try
    {
        logger.LogInformation("Testing database connection...");
        if (await dbContext.Database.CanConnectAsync())
        {
            logger.LogInformation("Database connection successful!");
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied.");
        }
        else
        {
            logger.LogWarning("Could not connect to database. Please check your configuration in Configuration/ENV_SETUP.cs");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database connection failed: {Message}", ex.Message);
        logger.LogWarning("The application will continue, but database operations will fail until the connection is fixed.");
    }
}

// Servir Blazor WASM desde wwwroot (copiado por el Dockerfile)
var mimeProvider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
mimeProvider.Mappings[".dat"] = "application/octet-stream";
mimeProvider.Mappings[".wasm"] = "application/wasm";
mimeProvider.Mappings[".blat"] = "application/octet-stream";
mimeProvider.Mappings[".webcil"] = "application/octet-stream";

app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = mimeProvider });

app.UseSwagger();

app.MapScalarApiReference("/docs", options =>
{
    options
        .WithTitle("RugbyEngine API Documentation")
        .WithTheme(ScalarTheme.Purple)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
        .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
});


// Leer headers de Caddy/nginx
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseJwtRefresh();
app.UseAuthorization();

app.MapControllers();

// SPA fallback: rutas del cliente Blazor que no coinciden con la API devuelven index.html
app.MapFallbackToFile("index.html");

app.Run();

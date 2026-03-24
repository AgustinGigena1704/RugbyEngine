using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data;

namespace RugbyEngine.Api.Services
{
    public static class DbConfigurationService
    {
        public static void ConfigureProduction(WebApplicationBuilder builder)
        {
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
        }

        public static void ConfigureDevelopment(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ApiDbContext>(opt =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' no encontrada.");
                opt.UseLazyLoadingProxies()
                   .UseNpgsql(connectionString);
            });
        }
    }
}

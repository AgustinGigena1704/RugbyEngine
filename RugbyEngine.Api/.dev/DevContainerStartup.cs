using System.Diagnostics;

namespace RugbyEngine.Api.Dev;

/// <summary>
/// Auto-inicia el contenedor PostgreSQL de desarrollo al arrancar la API.
/// Solo se usa cuando ASPNETCORE_ENVIRONMENT = Development.
/// 
/// Agregar en Program.cs antes de builder.Build():
/// 
///   if (builder.Environment.IsDevelopment())
///   {
///       var devLogger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger("Dev");
///       DevContainerStartup.EnsureRunning(devLogger);
///   }
/// </summary>
internal static class DevContainerStartup
{
    // Ruta relativa al docker-compose desde el binario compilado (bin/Debug/net10.0/)
    private static readonly string ComposeFilePath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".dev", "docker", "docker-compose.yml")
    );

    private const string ContainerName = "rugbyengine_dev_db";

    public static void EnsureRunning(ILogger logger)
    {
        try
        {
            if (!File.Exists(ComposeFilePath))
            {
                logger.LogWarning("[Dev] No se encontró docker-compose.yml en: {Path}", ComposeFilePath);
                return;
            }

            logger.LogInformation("[Dev] Iniciando contenedor PostgreSQL de desarrollo...");
            Execute("docker", $"compose -f \"{ComposeFilePath}\" up -d", logger);
            WaitUntilHealthy(logger);

            logger.LogInformation("[Dev] PostgreSQL listo en localhost:5433");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "[DEV] No se pudo iniciar el contenedor Docker. " +
                "Asegurate de que Docker Desktop esté corriendo o levantalo manualmente con: .\\dev.ps1 db setup");
        }
    }

    // ────────────────────────────────────────────────────────

    private static void WaitUntilHealthy(ILogger logger, int maxAttempts = 20)
    {
        for (var i = 1; i <= maxAttempts; i++)
        {
            var status = ExecuteWithOutput("docker",
                $"inspect --format={{{{.State.Health.Status}}}} {ContainerName}");

            if (status.Trim() == "healthy")
                return;

            logger.LogInformation("[Dev] Esperando PostgreSQL... ({I}/{Max}) — estado: {Status}",
                i, maxAttempts, status.Trim());

            Thread.Sleep(2_000);
        }

        throw new TimeoutException(
            $"El contenedor '{ContainerName}' no pasó a estado 'healthy' en el tiempo esperado.");
    }

    private static void Execute(string cmd, string args, ILogger logger)
    {
        using var p = Start(cmd, args, redirectOutput: false);
        p.WaitForExit();

        if (p.ExitCode != 0)
            logger.LogWarning("[DEV] '{Cmd} {Args}' finalizó con código {Code}", cmd, args, p.ExitCode);
    }

    private static string ExecuteWithOutput(string cmd, string args)
    {
        using var p = Start(cmd, args, redirectOutput: true);
        var output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();
        return output;
    }

    private static Process Start(string cmd, string args, bool redirectOutput)
    {
        var psi = new ProcessStartInfo(cmd, args)
        {
            UseShellExecute = false,
            RedirectStandardOutput = redirectOutput,
            RedirectStandardError = false,
            CreateNoWindow = true
        };
        return Process.Start(psi) ?? throw new InvalidOperationException($"No se pudo ejecutar: {cmd} {args}");
    }
}

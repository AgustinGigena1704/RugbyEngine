using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RugbyEngine.Shared.Health;

namespace RugbyEngine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
            /// Health check endpoint para verificar que la API está funcionando
            /// </summary>
            /// <remarks>
            /// Este endpoint retorna el estado de salud de la aplicación.
            /// 
            /// **Uso:**
            /// - Monitoreo en Render (health check automático)
            /// - Verificación manual de disponibilidad
            /// - Testing de conectividad
            /// 
            /// **No requiere autenticación.**
            /// </remarks>
            /// <response code="200">La API está saludable y funcionando correctamente</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(new HealthResponse
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Service = "RugbyEngine",
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }
    }
}


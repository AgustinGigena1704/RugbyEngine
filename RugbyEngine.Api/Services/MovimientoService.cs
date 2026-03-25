using RugbyEngine.Api.Data.Attributes;
using RugbyEngine.Api.Services.Interfaces;

namespace RugbyEngine.Api.Services
{
    [Service(ServiceLifetime.Singleton)]
    public class MovimientoService : IMovimientoService
    {
        private readonly ILogger<MovimientoService> logger;
        public MovimientoService(ILogger<MovimientoService> _logger)
        {
            logger = _logger;
        }


    }
}

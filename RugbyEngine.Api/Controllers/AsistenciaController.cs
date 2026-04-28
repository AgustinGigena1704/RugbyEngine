using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RugbyEngine.Api.Data;
using RugbyEngine.Api.Data.Repositories;
using RugbyEngine.Api.Services.Interfaces;
using RugbyEngine.Shared.Entrenamientos;

namespace RugbyEngine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AsistenciaController : GenericController
    {
        public AsistenciaController(EntityManager entityManager, ICurrentUserService currentUserService)
            : base(entityManager, currentUserService) { }

        [HttpGet("entrenamiento/{entrenamientoId:int}")]
        [ProducesResponseType(typeof(List<AsistenciaItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<AsistenciaItemDto>>> GetByEntrenamiento(int entrenamientoId, CancellationToken cancellationToken)
        {
            var entrenamiento = await _entityManager.GetRepository<EntrenamientoRepository>()
                .GetByIdAsync(entrenamientoId, cancellationToken: cancellationToken);
            if (entrenamiento == null) return NotFound();

            var jugadores = await _entityManager.GetRepository<JugadorRepository>()
                .GetByCategoriaAsync(entrenamiento.CategoriaId, cancellationToken);

            var asistencias = await _entityManager.GetRepository<AsistenciaRepository>()
                .GetByEntrenamientoAsync(entrenamientoId, cancellationToken);

            var result = jugadores.Select(j => new AsistenciaItemDto
            {
                JugadorId = j.Id,
                JugadorNombre = j.Persona.NombreCompleto,
                Estado = asistencias.FirstOrDefault(a => a.JugadorId == j.Id)?.Estado ?? AsistenciaEstado.Ausente
            }).ToList();

            return Ok(result);
        }

        [HttpPost("entrenamiento/{entrenamientoId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GuardarAsistencia(int entrenamientoId, [FromBody] AsistenciaBulkRequest request, CancellationToken cancellationToken)
        {
            var usuario = await GetCurrentUserAsync();
            if (usuario == null) return Unauthorized();

            var entrenamiento = await _entityManager.GetRepository<EntrenamientoRepository>()
                .GetByIdAsync(entrenamientoId, cancellationToken: cancellationToken);
            if (entrenamiento == null) return NotFound();

            await _entityManager.GetRepository<AsistenciaRepository>()
                .UpsertBulkAsync(entrenamientoId, request.Items, usuario, cancellationToken);

            return NoContent();
        }
    }
}

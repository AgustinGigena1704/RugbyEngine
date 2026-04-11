using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RugbyEngine.Api.Data;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Api.Data.Repositories;
using RugbyEngine.Api.Services.Interfaces;
using RugbyEngine.Shared.Entrenamientos;
using RugbyEngine.Shared.Tablas;

namespace RugbyEngine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EntrenamientoController : GenericController
    {
        public EntrenamientoController(EntityManager entityManager, ICurrentUserService currentUserService)
            : base(entityManager, currentUserService) { }

        [HttpGet]
        [ProducesResponseType(typeof(List<EntrenamientoResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EntrenamientoResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var items = await _entityManager.GetRepository<EntrenamientoRepository>()
                .GetAllAsync(cancellationToken: cancellationToken);
            return Ok(items.Select(MapToResponse).ToList());
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(List<EntrenamientoResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EntrenamientoResponse>>> Search([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var repo = _entityManager.GetRepository<EntrenamientoRepository>();
            var paginacion = new PaginacionDto { Pagina = page, RegistrosPorPagina = pageSize };
            var items = await repo.SearchAsync(search, paginacion, cancellationToken);
            return Ok(items.Select(MapToResponse).ToList());
        }

        [HttpGet("count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Count([FromQuery] string? search, CancellationToken cancellationToken)
        {
            var total = await _entityManager.GetRepository<EntrenamientoRepository>()
                .CountAsync(search, cancellationToken);
            return Ok(total);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EntrenamientoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EntrenamientoResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _entityManager.GetRepository<EntrenamientoRepository>()
                .GetByIdAsync(id, cancellationToken: cancellationToken);
            if (item == null) return NotFound();
            return Ok(MapToResponse(item));
        }

        [HttpPost]
        [ProducesResponseType(typeof(EntrenamientoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<EntrenamientoResponse>> Create([FromBody] EntrenamientoRequest request, CancellationToken cancellationToken)
        {
            var usuario = await GetCurrentUserAsync();
            if (usuario == null) return Unauthorized();

            var categoriaRepo = _entityManager.GetRepository<CategoriaRepository>();
            var categoria = await categoriaRepo.GetByIdAsync(request.CategoriaId, cancellationToken: cancellationToken);
            if (categoria == null) return BadRequest();

            var entity = new Entrenamiento
            {
                Categoria = categoria,
                CategoriaId = request.CategoriaId,
                Fecha = request.Fecha,
                CreatedBy = usuario,
                CreatedById = usuario.Id,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _entityManager.GetRepository<EntrenamientoRepository>().AddAsync(entity, usuario, cancellationToken);

            // Asegurar registros de asistencia para todos los jugadores de la categoría
            await _entityManager.GetRepository<AsistenciaRepository>()
                .EnsureForEntrenamientoAsync(created.Id, created.CategoriaId, usuario, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(created));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(EntrenamientoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EntrenamientoResponse>> Update(int id, [FromBody] EntrenamientoRequest request, CancellationToken cancellationToken)
        {
            var usuario = await GetCurrentUserAsync();
            if (usuario == null) return Unauthorized();

            var repo = _entityManager.GetRepository<EntrenamientoRepository>();
            var entity = await repo.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (entity == null) return NotFound();

            var categoriaRepo = _entityManager.GetRepository<CategoriaRepository>();
            var categoria = await categoriaRepo.GetByIdAsync(request.CategoriaId, cancellationToken: cancellationToken);
            if (categoria == null) return BadRequest();

            entity.Categoria = categoria;
            entity.CategoriaId = request.CategoriaId;
            entity.Fecha = request.Fecha;

            var updated = await repo.UpdateAsync(entity, usuario, cancellationToken);
            // Actualizar asistencias para reflejar la categoría y estados por defecto
            await _entityManager.GetRepository<AsistenciaRepository>()
                .EnsureForEntrenamientoAsync(updated.Id, updated.CategoriaId, usuario, cancellationToken);

            return Ok(MapToResponse(updated));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var usuario = await GetCurrentUserAsync();
            if (usuario == null) return Unauthorized();

            var deleted = await _entityManager.GetRepository<EntrenamientoRepository>()
                .DeleteAsync(id, usuario, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }

        private static EntrenamientoResponse MapToResponse(Entrenamiento e) => new()
        {
            Id = e.Id,
            CategoriaId = e.CategoriaId,
            CategoriaNombre = e.Categoria.Nombre,
            Fecha = e.Fecha
        };
    }
}

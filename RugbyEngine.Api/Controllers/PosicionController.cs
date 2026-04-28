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
    public class PosicionController : GenericController
    {
        public PosicionController(EntityManager entityManager, ICurrentUserService currentUserService)
            : base(entityManager, currentUserService) { }

        [HttpGet]
        [ProducesResponseType(typeof(List<PosicionResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PosicionResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var items = await _entityManager.GetRepository<PosicionRepository>()
                .GetAllAsync(cancellationToken: cancellationToken);
            return Ok(items.Select(MapToResponse).ToList());
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(List<PosicionResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PosicionResponse>>> Search([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var repo = _entityManager.GetRepository<PosicionRepository>();
            var paginacion = new PaginacionDto { Pagina = page, RegistrosPorPagina = pageSize };
            var items = await repo.SearchAsync(search, paginacion, cancellationToken);
            return Ok(items.Select(MapToResponse).ToList());
        }

        [HttpGet("count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Count([FromQuery] string? search, CancellationToken cancellationToken)
        {
            var total = await _entityManager.GetRepository<PosicionRepository>()
                .CountAsync(search, cancellationToken);
            return Ok(total);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PosicionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PosicionResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _entityManager.GetRepository<PosicionRepository>()
                .GetByIdAsync(id, cancellationToken: cancellationToken);
            if (item == null) return NotFound();
            return Ok(MapToResponse(item));
        }

        [HttpPost]
        [ProducesResponseType(typeof(PosicionResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PosicionResponse>> Create([FromBody] PosicionRequest request, CancellationToken cancellationToken)
        {
            var usuario = await GetCurrentUserAsync();
            if (usuario == null) return Unauthorized();

            var entity = new Posicion
            {
                Nombre = request.Nombre,
                Numero = request.Numero,
                CreatedBy = usuario,
                CreatedById = usuario.Id,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _entityManager.GetRepository<PosicionRepository>().AddAsync(entity, usuario, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(created));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(PosicionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PosicionResponse>> Update(int id, [FromBody] PosicionRequest request, CancellationToken cancellationToken)
        {
            var usuario = await GetCurrentUserAsync();
            if (usuario == null) return Unauthorized();

            var repo = _entityManager.GetRepository<PosicionRepository>();
            var entity = await repo.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (entity == null) return NotFound();

            entity.Nombre = request.Nombre;
            entity.Numero = request.Numero;

            var updated = await repo.UpdateAsync(entity, usuario, cancellationToken);
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

            var deleted = await _entityManager.GetRepository<PosicionRepository>()
                .DeleteAsync(id, usuario, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }

        private static PosicionResponse MapToResponse(Posicion p) => new()
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Numero = p.Numero
        };
    }
}

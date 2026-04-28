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
    public class CategoriaController : GenericController
    {
        public CategoriaController(EntityManager entityManager, ICurrentUserService currentUserService)
            : base(entityManager, currentUserService) { }

        [HttpGet]
        [ProducesResponseType(typeof(List<CategoriaResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CategoriaResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var items = await _entityManager.GetRepository<CategoriaRepository>()
                .GetAllAsync(cancellationToken: cancellationToken);
            return Ok(items.Select(MapToResponse).ToList());
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(List<CategoriaResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CategoriaResponse>>> Search([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var repo = _entityManager.GetRepository<CategoriaRepository>();
            var paginacion = new PaginacionDto { Pagina = page, RegistrosPorPagina = pageSize };
            var items = await repo.SearchAsync(search, paginacion, cancellationToken);
            return Ok(items.Select(MapToResponse).ToList());
        }

        [HttpGet("count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Count([FromQuery] string? search, CancellationToken cancellationToken)
        {
            var total = await _entityManager.GetRepository<CategoriaRepository>()
                .CountAsync(search, cancellationToken);
            return Ok(total);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _entityManager.GetRepository<CategoriaRepository>()
                .GetByIdAsync(id, cancellationToken: cancellationToken);
            if (item == null) return NotFound();
            return Ok(MapToResponse(item));
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CategoriaResponse>> Create([FromBody] CategoriaRequest request, CancellationToken cancellationToken)
        {
            var usuario = await GetCurrentUserAsync();
            if (usuario == null) return Unauthorized();

            var entity = new Categoria
            {
                Nombre = request.Nombre,
                Abreviatura = request.Abreviatura,
                EdadMinima = request.EdadMinima,
                EdadMaxima = request.EdadMaxima,
                CreatedBy = usuario,
                CreatedById = usuario.Id,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _entityManager.GetRepository<CategoriaRepository>().AddAsync(entity, usuario, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(created));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaResponse>> Update(int id, [FromBody] CategoriaRequest request, CancellationToken cancellationToken)
        {
            var usuario = await GetCurrentUserAsync();
            if (usuario == null) return Unauthorized();

            var repo = _entityManager.GetRepository<CategoriaRepository>();
            var entity = await repo.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (entity == null) return NotFound();

            entity.Nombre = request.Nombre;
            entity.Abreviatura = request.Abreviatura;
            entity.EdadMinima = request.EdadMinima;
            entity.EdadMaxima = request.EdadMaxima;

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

            var deleted = await _entityManager.GetRepository<CategoriaRepository>()
                .DeleteAsync(id, usuario, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }

        private static CategoriaResponse MapToResponse(Categoria c) => new()
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Abreviatura = c.Abreviatura,
            EdadMinima = c.EdadMinima,
            EdadMaxima = c.EdadMaxima
        };
    }
}

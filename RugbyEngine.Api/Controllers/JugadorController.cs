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
    public class JugadorController : GenericController
    {
        public JugadorController(EntityManager entityManager, ICurrentUserService currentUserService)
            : base(entityManager, currentUserService) { }

        [HttpGet]
        [ProducesResponseType(typeof(List<JugadorResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<JugadorResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var items = await _entityManager.GetRepository<JugadorRepository>()
                .GetAllAsync(cancellationToken: cancellationToken);
            return Ok(items.Select(MapToResponse).ToList());
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(List<JugadorResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<JugadorResponse>>> Search([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var repo = _entityManager.GetRepository<JugadorRepository>();
            var paginacion = new PaginacionDto { Pagina = page, RegistrosPorPagina = pageSize };
            var items = await repo.SearchAsync(search, paginacion, cancellationToken);
            return Ok(items.Select(MapToResponse).ToList());
        }

        [HttpGet("count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Count([FromQuery] string? search, CancellationToken cancellationToken)
        {
            var total = await _entityManager.GetRepository<JugadorRepository>()
                .CountAsync(search, cancellationToken);
            return Ok(total);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(JugadorResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JugadorResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _entityManager.GetRepository<JugadorRepository>()
                .GetByIdAsync(id, cancellationToken: cancellationToken);
            if (item == null) return NotFound();
            return Ok(MapToResponse(item));
        }

        [HttpPost]
        [ProducesResponseType(typeof(JugadorResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<JugadorResponse>> Create([FromBody] JugadorRequest request, CancellationToken cancellationToken)
        {
            var usuario = await GetCurrentUserAsync();
            if (usuario == null) return Unauthorized();

            var personaRepo = _entityManager.GetRepository<PersonaRepository>();
            var persona = await personaRepo.GetByIdAsync(request.PersonaId, cancellationToken: cancellationToken);
            if (persona == null) return BadRequest();

            var categoriaRepo = _entityManager.GetRepository<CategoriaRepository>();
            var categoria = await categoriaRepo.GetByIdAsync(request.CategoriaId, cancellationToken: cancellationToken);
            if (categoria == null) return BadRequest();

            var posicionRepo = _entityManager.GetRepository<PosicionRepository>();
            var posicionPrincipal = await posicionRepo.GetByIdAsync(request.PosicionPrincipalId, cancellationToken: cancellationToken);
            if (posicionPrincipal == null) return BadRequest();

            var entity = new Jugador
            {
                Persona = persona,
                PersonaId = request.PersonaId,
                Categoria = categoria,
                CategoriaId = request.CategoriaId,
                PosicionPrincipal = posicionPrincipal,
                PosicionPrincipalId = request.PosicionPrincipalId,
                PosicionSecundariaId = request.PosicionSecundariaId,
                PosicionTerciariaId = request.PosicionTerciariaId,
                CreatedBy = usuario,
                CreatedById = usuario.Id,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _entityManager.GetRepository<JugadorRepository>().AddAsync(entity, usuario, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(created));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(JugadorResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JugadorResponse>> Update(int id, [FromBody] JugadorRequest request, CancellationToken cancellationToken)
        {
            var usuario = await GetCurrentUserAsync();
            if (usuario == null) return Unauthorized();

            var repo = _entityManager.GetRepository<JugadorRepository>();
            var entity = await repo.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (entity == null) return NotFound();

            var personaRepo = _entityManager.GetRepository<PersonaRepository>();
            var persona = await personaRepo.GetByIdAsync(request.PersonaId, cancellationToken: cancellationToken);
            if (persona == null) return BadRequest();

            var categoriaRepo = _entityManager.GetRepository<CategoriaRepository>();
            var categoria = await categoriaRepo.GetByIdAsync(request.CategoriaId, cancellationToken: cancellationToken);
            if (categoria == null) return BadRequest();

            var posicionRepo = _entityManager.GetRepository<PosicionRepository>();
            var posicionPrincipal = await posicionRepo.GetByIdAsync(request.PosicionPrincipalId, cancellationToken: cancellationToken);
            if (posicionPrincipal == null) return BadRequest();

            entity.Persona = persona;
            entity.PersonaId = request.PersonaId;
            entity.Categoria = categoria;
            entity.CategoriaId = request.CategoriaId;
            entity.PosicionPrincipal = posicionPrincipal;
            entity.PosicionPrincipalId = request.PosicionPrincipalId;
            entity.PosicionSecundariaId = request.PosicionSecundariaId;
            entity.PosicionTerciariaId = request.PosicionTerciariaId;

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

            var deleted = await _entityManager.GetRepository<JugadorRepository>()
                .DeleteAsync(id, usuario, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }

        private static JugadorResponse MapToResponse(Jugador j) => new()
        {
            Id = j.Id,
            PersonaId = j.PersonaId,
            NombreCompleto = j.Persona.NombreCompleto,
            CategoriaId = j.CategoriaId,
            CategoriaNombre = j.Categoria.Nombre,
            PosicionPrincipalId = j.PosicionPrincipalId,
            PosicionPrincipalNombre = j.PosicionPrincipal.Nombre,
            PosicionSecundariaId = j.PosicionSecundariaId,
            PosicionSecundariaNombre = j.PosicionSecundaria?.Nombre,
            PosicionTerciariaId = j.PosicionTerciariaId,
            PosicionTerciariaNombre = j.PosicionTerciaria?.Nombre
        };
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RugbyEngine.Api.Data;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Api.Data.Repositories;
using RugbyEngine.Api.Services.Interfaces;
using RugbyEngine.Shared;
using RugbyEngine.Shared.Personas;
using RugbyEngine.Shared.Tablas;

namespace RugbyEngine.Api.Controllers
{
    /// <summary>
    /// Controlador para gestión de personas
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PersonaController : GenericController
    {
        /// <summary>
        /// Constructor del controlador de personas
        /// </summary>
        public PersonaController(EntityManager entityManager, ICurrentUserService currentUserService)
            : base(entityManager, currentUserService)
        {
        }

        /// <summary>
        /// Obtiene todas las personas activas.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<PersonaResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PersonaResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var personas = await _entityManager.GetRepository<PersonaRepository>()
                .GetAllAsync(cancellationToken: cancellationToken);

            return Ok(personas.Select(MapToResponse).ToList());
        }

        /// <summary>
        /// Obtiene personas activas paginadas.
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<PersonaResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PersonaResponse>>> Search([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var repo = _entityManager.GetRepository<PersonaRepository>();
            var paginacion = new PaginacionDto { Pagina = page, RegistrosPorPagina = pageSize };
            var personas = await repo.SearchAsync(search, paginacion, cancellationToken);

            return Ok(personas.Select(MapToResponse).ToList());
        }

        /// <summary>
        /// Devuelve el total de personas activas para un filtro.
        /// </summary>
        [HttpGet("count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Count([FromQuery] string? search, CancellationToken cancellationToken)
        {
            var total = await _entityManager.GetRepository<PersonaRepository>()
                .CountAsync(search, cancellationToken);

            return Ok(total);
        }

        /// <summary>
        /// Obtiene una persona por su ID
        /// </summary>
        /// <response code="200">Persona encontrada</response>
        /// <response code="404">Persona no encontrada</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PersonaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PersonaResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var persona = await _entityManager.GetRepository<PersonaRepository>()
                .GetByIdAsync(id, cancellationToken: cancellationToken);

            if (persona == null)
                return NotFound();

            return Ok(MapToResponse(persona));
        }

        /// <summary>
        /// Crea una nueva persona
        /// </summary>
        /// <response code="201">Persona creada</response>
        /// <response code="400">Datos inválidos o documento ya existente</response>
        /// <response code="401">No autorizado</response>
        [HttpPost]
        [ProducesResponseType(typeof(PersonaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PersonaResponse>> Create([FromBody] PersonaRequest request, CancellationToken cancellationToken)
        {
            var usuario = await GetCurrentUserAsync();
            if (usuario == null)
                return Unauthorized();

            var repo = _entityManager.GetRepository<PersonaRepository>();

            if (await repo.ExistsDocumentoAsync(request.Documento, cancellationToken: cancellationToken))
                return BadRequest(new ApiResponse { Success = false, Message = $"Ya existe una persona con el documento '{request.Documento}'." });

            var persona = new Persona
            {
                Nombres = request.Nombres,
                Apellidos = request.Apellidos,
                Documento = request.Documento,
                FechaNacimiento = request.FechaNacimiento,
                Cobertura = request.Cobertura,
                NroAfiliado = request.NroAfiliado,
                Telefono = request.Telefono,
                Email = request.Email,
                Domicilio = request.Domicilio,
                CreatedBy = usuario,
                CreatedById = usuario.Id,
                CreatedAt = DateTime.UtcNow
            };

            var created = await repo.AddAsync(persona, usuario, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(created));
        }

        /// <summary>
        /// Actualiza una persona existente
        /// </summary>
        /// <response code="200">Persona actualizada</response>
        /// <response code="400">Documento ya utilizado por otra persona</response>
        /// <response code="401">No autorizado</response>
        /// <response code="404">Persona no encontrada</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(PersonaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PersonaResponse>> Update(int id, [FromBody] PersonaRequest request, CancellationToken cancellationToken)
        {
            var usuario = await GetCurrentUserAsync();
            if (usuario == null)
                return Unauthorized();

            var repo = _entityManager.GetRepository<PersonaRepository>();

            var persona = await repo.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (persona == null)
                return NotFound();

            var documentoEnUso = await repo.GetByDocumentoAsync(request.Documento, cancellationToken: cancellationToken);
            if (documentoEnUso != null && documentoEnUso.Id != id)
                return BadRequest(new ApiResponse { Success = false, Message = $"El documento '{request.Documento}' ya está en uso por otra persona." });

            persona.Nombres = request.Nombres;
            persona.Apellidos = request.Apellidos;
            persona.Documento = request.Documento;
            persona.FechaNacimiento = request.FechaNacimiento;
            persona.Cobertura = request.Cobertura;
            persona.NroAfiliado = request.NroAfiliado;
            persona.Telefono = request.Telefono;
            persona.Email = request.Email;
            persona.Domicilio = request.Domicilio;

            var updated = await repo.UpdateAsync(persona, usuario, cancellationToken);
            return Ok(MapToResponse(updated));
        }

        /// <summary>
        /// Elimina lógicamente una persona
        /// </summary>
        /// <response code="200">Persona eliminada</response>
        /// <response code="401">No autorizado</response>
        /// <response code="404">Persona no encontrada</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> Delete(int id, CancellationToken cancellationToken)
        {
            var usuario = await GetCurrentUserAsync();
            if (usuario == null)
                return Unauthorized();

            var deleted = await _entityManager.GetRepository<PersonaRepository>()
                .DeleteAsync(id, usuario, cancellationToken);

            if (!deleted)
                return NotFound();

            return Ok(new ApiResponse { Success = true, Message = "Persona eliminada correctamente." });
        }

        private static PersonaResponse MapToResponse(Persona p) => new()
        {
            Id = p.Id,
            Nombres = p.Nombres,
            Apellidos = p.Apellidos,
            Documento = p.Documento,
            FechaNacimiento = p.FechaNacimiento,
            Cobertura = p.Cobertura,
            NroAfiliado = p.NroAfiliado,
            Telefono = p.Telefono,
            Email = p.Email,
            Domicilio = p.Domicilio
        };

        [HttpGet("disponibles")]
        [ProducesResponseType(typeof(List<PersonaResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PersonaResponse>>> GetDisponibles([FromQuery] string? search, [FromQuery] int pageSize = 10, [FromQuery] int? incluirPersonaId = null, CancellationToken cancellationToken = default)
        {
            var personas = await _entityManager.GetRepository<PersonaRepository>()
                .SearchDisponiblesAsync(search, pageSize, incluirPersonaId, cancellationToken);
            return Ok(personas.Select(MapToResponse).ToList());
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RugbyEngine.Api.Data;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Api.Data.Repositories;
using RugbyEngine.Api.Services;
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
        /// Obtiene todas las personas activas
        /// </summary>
        /// <response code="200">Lista de personas</response>
        [HttpGet]
        [ProducesResponseType(typeof(TableResponse<PersonaResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<TableResponse<PersonaResponse>>> GetAll([FromQuery] PaginacionDto paginacion, CancellationToken cancellationToken)
        {
            return Ok(await GetTableResponseAsync(paginacion, cancellationToken));
        }

        [HttpPost("table")]
        [ProducesResponseType(typeof(TableResponse<PersonaResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<TableResponse<PersonaResponse>>> GetTable([FromBody] PaginacionDto paginacion, CancellationToken cancellationToken)
        {
            return Ok(await GetTableResponseAsync(paginacion, cancellationToken));
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

        private async Task<TableResponse<PersonaResponse>> GetTableResponseAsync(PaginacionDto paginacion, CancellationToken cancellationToken)
        {
            paginacion.Pagina = paginacion.Pagina < 1 ? 1 : paginacion.Pagina;
            paginacion.RegistrosPorPagina = paginacion.RegistrosPorPagina < 1 ? 10 : paginacion.RegistrosPorPagina;

            var repo = _entityManager.GetRepository<PersonaRepository>();

            var totalRegistros = (await repo.GetAllAsync(cancellationToken: cancellationToken)).Count();
            var personas = await repo.GetAllAsync(paginacion: paginacion, cancellationToken: cancellationToken);

            paginacion.Total = totalRegistros;

            return new TableResponse<PersonaResponse>
            {
                Paginacion = paginacion,
                Registros = personas.Select(MapToResponse).ToList()
            };
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
    }
}

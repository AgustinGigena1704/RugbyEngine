using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RugbyEngine.Api.Data;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Api.Data.Repositories;
using RugbyEngine.Api.Services;
using RugbyEngine.Shared;
using RugbyEngine.Shared.Perfiles;
using RugbyEngine.Shared.Tablas;
using RugbyEngine.Shared.Usuarios;

namespace RugbyEngine.Api.Controllers
{
    /// <summary>Controlador para gestión de usuarios.</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsuarioController : GenericController
    {
        private readonly IPasswordHasher _passwordHasher;

        public UsuarioController(EntityManager entityManager, ICurrentUserService currentUserService, IPasswordHasher passwordHasher)
            : base(entityManager, currentUserService)
        {
            _passwordHasher = passwordHasher;
        }

        /// <summary>Devuelve todos los usuarios activos con su persona y perfiles.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<UsuarioResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UsuarioResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var usuarios = await _entityManager.GetRepository<UsuarioRepository>()
                .GetAllAsync(cancellationToken: cancellationToken);

            return Ok(usuarios.Select(MapToResponse).ToList());
        }

        /// <summary>Devuelve usuarios paginados para grillas/tablas.</summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<UsuarioResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UsuarioResponse>>> Search([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var repo = _entityManager.GetRepository<UsuarioRepository>();
            var paginacion = new PaginacionDto { Pagina = page, RegistrosPorPagina = pageSize };
            var usuarios = await repo.SearchAsync(search, paginacion, cancellationToken);

            return Ok(usuarios.Select(MapToResponse).ToList());
        }

        /// <summary>Devuelve el total de usuarios activos.</summary>
        [HttpGet("count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Count([FromQuery] string? search, CancellationToken cancellationToken)
        {
            var total = await _entityManager.GetRepository<UsuarioRepository>()
                .CountAsync(search, cancellationToken);

            return Ok(total);
        }

        /// <summary>Devuelve un usuario por su id con persona y perfiles.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var usuario = await _entityManager.GetRepository<UsuarioRepository>()
                .GetByIdAsync(id, cancellationToken: cancellationToken);

            if (usuario == null)
                return NotFound();

            return Ok(MapToResponse(usuario));
        }

        /// <summary>Crea un nuevo usuario. Valida username único y persona sin usuario previo.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UsuarioResponse>> Create([FromBody] UsuarioCreateDto dto, CancellationToken cancellationToken)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var repo = _entityManager.GetRepository<UsuarioRepository>();

            if (await repo.ExistsAsync(dto.Username, cancellationToken: cancellationToken))
                return BadRequest(new ApiResponse { Success = false, Message = $"El usuario '{dto.Username}' ya existe." });

            if (await repo.PersonaYaTieneUsuarioAsync(dto.PersonaId, cancellationToken: cancellationToken))
                return BadRequest(new ApiResponse { Success = false, Message = "La persona seleccionada ya tiene un usuario asignado." });

            var nuevo = new Usuario
            {
                Username = dto.Username,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                Email = dto.Email,
                PersonaId = dto.PersonaId,
                LastLogin = DateTime.UtcNow,
                CreatedBy = currentUser,
                CreatedById = currentUser.Id,
                CreatedAt = DateTime.UtcNow
            };

            var created = await repo.AddAsync(nuevo, currentUser, cancellationToken);
            var withDetails = await repo.GetByIdAsync(created.Id, cancellationToken: cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(withDetails!));
        }

        /// <summary>Actualiza username y email de un usuario.</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioResponse>> Update(int id, [FromBody] UsuarioUpdateDto dto, CancellationToken cancellationToken)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var repo = _entityManager.GetRepository<UsuarioRepository>();

            var usuario = await repo.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (usuario == null)
                return NotFound();

            var existente = await repo.GetByUsernameAsync(dto.Username, cancellationToken: cancellationToken);
            if (existente != null && existente.Id != id)
                return BadRequest(new ApiResponse { Success = false, Message = $"El usuario '{dto.Username}' ya está en uso." });

            usuario.Username = dto.Username;
            usuario.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                usuario.PasswordHash = _passwordHasher.HashPassword(dto.Password);
            }

            var updated = await repo.UpdateAsync(usuario, currentUser, cancellationToken);
            var withDetails = await repo.GetByIdAsync(updated.Id, cancellationToken: cancellationToken);
            return Ok(MapToResponse(withDetails!));
        }

        /// <summary>Elimina lógicamente un usuario.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> Delete(int id, CancellationToken cancellationToken)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var deleted = await _entityManager.GetRepository<UsuarioRepository>()
                .DeleteAsync(id, currentUser, cancellationToken);

            if (!deleted)
                return NotFound();

            return Ok(new ApiResponse { Success = true, Message = "Usuario eliminado correctamente." });
        }

        /// <summary>Asigna un perfil a un usuario específico.</summary>
        [HttpPost("{usuarioId:int}/perfiles/{perfilId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignPerfil(int usuarioId, int perfilId, CancellationToken cancellationToken)
        {
            var usuario = await _entityManager.GetRepository<UsuarioRepository>()
                .GetByIdAsync(usuarioId, cancellationToken: cancellationToken);

            if (usuario == null)
                return NotFound(new ApiResponse { Success = false, Message = "Usuario no encontrado." });

            await _entityManager.GetRepository<PerfilRepository>()
                .AssignToUserAsync(perfilId, usuarioId, cancellationToken);

            return Ok();
        }

        /// <summary>Retira un perfil de un usuario específico.</summary>
        [HttpDelete("{usuarioId:int}/perfiles/{perfilId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnassignPerfil(int usuarioId, int perfilId, CancellationToken cancellationToken)
        {
            var usuario = await _entityManager.GetRepository<UsuarioRepository>()
                .GetByIdAsync(usuarioId, cancellationToken: cancellationToken);

            if (usuario == null)
                return NotFound(new ApiResponse { Success = false, Message = "Usuario no encontrado." });

            await _entityManager.GetRepository<PerfilRepository>()
                .RemoveFromUserAsync(perfilId, usuarioId, cancellationToken);

            return Ok();
        }

        private static UsuarioResponse MapToResponse(Usuario u) => new()
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            LastLogin = u.LastLogin,
            Activo = !u.BorradoLogico,
            PersonaId = u.PersonaId,
            NombreCompleto = u.Persona != null
                ? $"{u.Persona.Apellidos}, {u.Persona.Nombres}"
                : string.Empty,
            Documento = u.Persona?.Documento ?? string.Empty,
            Perfiles = u.Perfiles?
                .Select(p => new PerfilDto { Id = p.Id, Nombre = p.Nombre, Descripcion = p.Descripcion })
                .ToList() ?? []
        };
    }
}

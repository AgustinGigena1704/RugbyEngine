using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RugbyEngine.Api.Data;
using RugbyEngine.Api.Data.Repositories;
using RugbyEngine.Api.Services;
using RugbyEngine.Shared.Perfiles;

namespace RugbyEngine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PerfilController : GenericController
    {
        public PerfilController(EntityManager entityManager, ICurrentUserService currentUserService)
            : base(entityManager, currentUserService)
        {
        }

        /// <summary>Devuelve todos los perfiles disponibles. Requiere permiso ADMIN.</summary>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            var perfiles = await _entityManager.GetRepository<PerfilRepository>().GetAllAsync();
            return Ok(perfiles.Select(p => new PerfilDTO { Id = p.Id, Nombre = p.Nombre, Descripcion = p.Descripcion }));
        }

        /// <summary>Devuelve los perfiles asignados al usuario autenticado.</summary>
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return NotFound();

            var perfiles = await _entityManager.GetRepository<PerfilRepository>().GetByUserIdAsync(user.Id);
            return Ok(perfiles.Select(p => new PerfilDTO { Id = p.Id, Nombre = p.Nombre, Descripcion = p.Descripcion }));
        }

        /// <summary>Asigna un perfil al usuario autenticado. Requiere permiso ADMIN.</summary>
        [HttpPost("{id:int}/assign")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Assign(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return NotFound();

            await _entityManager.GetRepository<PerfilRepository>().AssignToUserAsync(id, user.Id);
            return Ok();
        }

        /// <summary>Retira un perfil del usuario autenticado. Requiere permiso ADMIN.</summary>
        [HttpDelete("{id:int}/unassign")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Unassign(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return NotFound();

            await _entityManager.GetRepository<PerfilRepository>().RemoveFromUserAsync(id, user.Id);
            return Ok();
        }
    }
}

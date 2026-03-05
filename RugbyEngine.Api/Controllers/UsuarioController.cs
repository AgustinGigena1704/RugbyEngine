using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RugbyEngine.Api.Data;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Api.Data.Repositories;
using RugbyEngine.Api.Services;

namespace RugbyEngine.Api.Controllers
{
    /// <summary>
    /// Controlador para gestión de usuarios
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : GenericController
    {
        /// <summary>
        /// Constructor del controlador de usuarios
        /// </summary>
        /// <param name="entityManager">Gestor de entidades</param>
        /// <param name="currentUserService">Servicio del usuario actual</param>
        public UsuarioController(EntityManager entityManager, ICurrentUserService currentUserService)
            : base(entityManager, currentUserService)
        {
        }
    }
}



using RugbyEngine.Api.Data.Attributes;
using RugbyEngine.Api.Data.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace RugbyEngine.Api.Data.Entities
{
    [Repository(typeof(UsuarioRepository))]
    [Table("Usuario")]
    public class Usuario : GenericEntity, IAudithory
    {
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime LastLogin { get; set; } = DateTime.UtcNow;
        public virtual Persona? Persona { get; set; }
        public int? PersonaId { get; set; }
        public virtual required Usuario CreatedBy { get; set; }
        public required int CreatedById { get; set; }
        public required DateTime CreatedAt { get; set; }
        public virtual Usuario? UpdatedBy { get; set; } = null;
        public int? UpdatedById { get; set; } = null;
        public DateTime? UpdatedAt { get; set; } = null;
        public virtual Usuario? DeletedBy { get; set; } = null;
        public int? DeletedById { get; set; } = null;
        public DateTime? DeletedAt { get; set; } = null;
        public virtual List<Perfil>? Perfiles { get; set; } = null;
    }
}

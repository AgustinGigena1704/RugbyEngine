using RugbyEngine.Api.Data.Attributes;
using RugbyEngine.Api.Data.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace RugbyEngine.Api.Data.Entities
{
    [Repository(typeof(MenuRepository))]
    [Table("Menu")]
    public class Menu : GenericEntity, IAudithory
    {
        public required string Titulo { get; set; }
        public string? ToolTip { get; set; } = null;
        public string? Icono { get; set; } = null;
        public string? Ruta { get; set; } = null;
        public virtual Menu? MenuPadre { get; set; }
        public int? MenuPadreId { get; set; }
        public int Lvl { get; set; } = 0;
        public int? PermisoId { get; set; }
        public required virtual Permiso? Permiso { get; set; }
        public required virtual Usuario CreatedBy { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Usuario? UpdatedBy { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual Usuario? DeletedBy { get; set; }
        public int? DeletedById { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}

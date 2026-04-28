using RugbyEngine.Api.Data.Attributes;
using RugbyEngine.Api.Data.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace RugbyEngine.Api.Data.Entities
{
    [Repository(typeof(PosicionRepository))]
    [Table("Posicion")]
    public class Posicion : GenericEntity, IAudithory
    {
        public required string Nombre { get; set; }
        public required int Numero { get; set; }
        public required virtual Usuario CreatedBy { get; set; }
        public required int CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Usuario? UpdatedBy { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual Usuario? DeletedBy { get; set; }
        public int? DeletedById { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}

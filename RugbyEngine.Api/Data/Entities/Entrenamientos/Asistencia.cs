using RugbyEngine.Api.Data.Attributes;
using RugbyEngine.Api.Data.Repositories;
using RugbyEngine.Shared.Entrenamientos;
using System.ComponentModel.DataAnnotations.Schema;

namespace RugbyEngine.Api.Data.Entities
{
    [Repository(typeof(AsistenciaRepository))]
    [Table("Asistencia")]
    public class Asistencia : GenericEntity, IAudithory
    {
        public required virtual Entrenamiento Entrenamiento { get; set; }
        public required int EntrenamientoId { get; set; }
        public required virtual Jugador Jugador { get; set; }
        public required int JugadorId { get; set; }
        public AsistenciaEstado Estado { get; set; }
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

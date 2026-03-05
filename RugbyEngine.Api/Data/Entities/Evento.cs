namespace RugbyEngine.Api.Data.Entities
{
    public class Evento : GenericEntity, IAudithory
    {
        public required string Nombre { get; set; }
        public DateOnly Fecha { get; set; }
        public required virtual TipoEvento TipoEvento { get; set; }
        public required int TipoEventoId { get; set; }
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

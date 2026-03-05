namespace RugbyEngine.Api.Data.Entities
{
    public class EventoMovimientos : GenericEntity, IAudithory
    {
        public required virtual Evento Evento { get; set; }
        public required int EventoId { get; set; }
        public required virtual Movimiento Movimiento { get; set; }
        public required int MovimientoId { get; set; }
        public required virtual Usuario CreatedBy { get; set; }
        public required int CreatedById { get; set; }
        public required DateTime CreatedAt { get; set; }
        public virtual Usuario? UpdatedBy { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual Usuario? DeletedBy { get; set; }
        public int? DeletedById { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}

namespace RugbyEngine.Api.Data.Entities
{
    public class EventoCuenta : GenericEntity, IAudithory
    {
        public required virtual Evento Evento { get; set; }
        public required int EventoId { get; set; }
        public required virtual Cuenta Cuenta { get; set; }
        public required int CuentaId { get; set; }
        public string Concepto { get; set; } = string.Empty;
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

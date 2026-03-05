namespace RugbyEngine.Api.Data.Entities
{
    public class MovimientoItem : GenericEntity, IAudithory
    {
        public required virtual Movimiento Movimiento { get; set; }
        public required int MovimientoId { get; set; }
        public required string Producto { get; set; }
        public decimal? Monto { get; set; }
        public string? Descripcion { get; set; }
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

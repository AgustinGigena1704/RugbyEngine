namespace RugbyEngine.Api.Data.Entities
{
    public class TipoEvento : GenericEntity, IAudithory
    {
        public required string Nombre { get; set; }
        public required string Codigo { get; set; }
        public string? Descripcion { get; set; } = null;
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

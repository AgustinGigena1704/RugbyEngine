namespace RugbyEngine.Api.Data.Entities
{
    public class Categoria : GenericEntity, IAudithory
    {
        public required string Nombre { get; set; }
        public required string Abreviatura { get; set; }
        public required int EdadMinima { get; set; } = 0;
        public required int EdadMaxima { get; set; } = 0;
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

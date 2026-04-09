namespace RugbyEngine.Api.Data.Entities
{
    public class Entrenamiento : GenericEntity, IAudithory
    {

        public virtual required Categoria Categoria { get; set; }
        public required int CategoriaId { get; set; }
        public required DateOnly Fecha { get; set; }
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

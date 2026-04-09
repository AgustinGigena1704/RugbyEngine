namespace RugbyEngine.Api.Data.Entities
{
    public class Jugador : GenericEntity, IAudithory
    {
        public required virtual Persona Persona { get; set; }
        public required int PersonaId { get; set; }
        public required virtual Categoria Categoria { get; set; }
        public required int CategoriaId { get; set; }
        public required virtual Posicion PosicionPrincipal { get; set; }
        public required int PosicionPrincipalId { get; set; }
        public virtual Posicion? PosicionSecundaria { get; set; } = null;
        public int? PosicionSecundariaId { get; set; } = null;
        public virtual Posicion? PosicionTerciaria { get; set; } = null;
        public int? PosicionTerciariaId { get; set; } = null;
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

namespace RugbyEngine.Api.Data.Entities
{
    public class TipoMovimiento : GenericEntity
    {
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; } = null;
    }
}

namespace RugbyEngine.Shared.Perfiles
{
    public class PerfilDTO
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}

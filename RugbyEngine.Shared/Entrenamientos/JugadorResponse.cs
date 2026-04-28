namespace RugbyEngine.Shared.Entrenamientos
{
    public class JugadorResponse
    {
        public int Id { get; set; }
        public int PersonaId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public int PosicionPrincipalId { get; set; }
        public string PosicionPrincipalNombre { get; set; } = string.Empty;
        public int? PosicionSecundariaId { get; set; }
        public string? PosicionSecundariaNombre { get; set; }
        public int? PosicionTerciariaId { get; set; }
        public string? PosicionTerciariaNombre { get; set; }
    }
}

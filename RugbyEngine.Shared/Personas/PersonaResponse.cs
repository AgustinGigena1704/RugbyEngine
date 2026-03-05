namespace RugbyEngine.Shared.Personas
{
    public class PersonaResponse
    {
        public int Id { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public DateOnly FechaNacimiento { get; set; }
        public string? Cobertura { get; set; }
        public string? NroAfiliado { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Domicilio { get; set; }
    }
}

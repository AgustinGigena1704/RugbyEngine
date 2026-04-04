using RugbyEngine.Shared.Perfiles;

namespace RugbyEngine.Shared.Usuarios
{
    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime LastLogin { get; set; }
        public bool Activo { get; set; }
        public int? PersonaId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public List<PerfilDto> Perfiles { get; set; } = [];
    }
}

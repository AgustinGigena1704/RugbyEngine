using System.ComponentModel.DataAnnotations;

namespace RugbyEngine.Shared.Usuarios
{
    public class UsuarioCreateDto
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [MaxLength(30, ErrorMessage = "Máximo 30 caracteres")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El email no es válido")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar una persona")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una persona")]
        public int PersonaId { get; set; }
    }
}

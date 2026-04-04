using System.ComponentModel.DataAnnotations;

namespace RugbyEngine.Shared.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;
    }
}

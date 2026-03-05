using System.ComponentModel.DataAnnotations;

namespace RugbyEngine.Shared.Personas
{
    public class PersonaRequest
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El documento es requerido")]
        [MaxLength(20, ErrorMessage = "Máximo 20 caracteres")]
        public string Documento { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateOnly FechaNacimiento { get; set; }

        [MaxLength(100)]
        public string? Cobertura { get; set; }

        [MaxLength(50)]
        public string? NroAfiliado { get; set; }

        [MaxLength(30)]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El email no es válido")]
        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(200)]
        public string? Domicilio { get; set; }
    }
}

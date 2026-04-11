using System.ComponentModel.DataAnnotations;

namespace RugbyEngine.Shared.Entrenamientos
{
    public class PosicionRequest
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número es requerido")]
        [Range(1, 999, ErrorMessage = "El número debe ser entre 1 y 999")]
        public int Numero { get; set; }
    }
}

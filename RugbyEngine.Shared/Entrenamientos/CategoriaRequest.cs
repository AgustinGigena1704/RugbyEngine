using System.ComponentModel.DataAnnotations;

namespace RugbyEngine.Shared.Entrenamientos
{
    public class CategoriaRequest
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La abreviatura es requerida")]
        [MaxLength(20, ErrorMessage = "Máximo 20 caracteres")]
        public string Abreviatura { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "La edad mínima debe estar entre 0 y 100")]
        public int EdadMinima { get; set; }

        [Range(0, 100, ErrorMessage = "La edad máxima debe estar entre 0 y 100")]
        public int EdadMaxima { get; set; }
    }
}

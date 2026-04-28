using System.ComponentModel.DataAnnotations;

namespace RugbyEngine.Shared.Entrenamientos
{
    public class EntrenamientoRequest
    {
        [Required(ErrorMessage = "La categoría es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría válida")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateOnly Fecha { get; set; }
    }
}

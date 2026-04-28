using System.ComponentModel.DataAnnotations;

namespace RugbyEngine.Shared.Entrenamientos
{
    public class JugadorRequest
    {
        [Required(ErrorMessage = "La persona es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una persona válida")]
        public int PersonaId { get; set; }

        [Required(ErrorMessage = "La categoría es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría válida")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "La posición principal es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una posición válida")]
        public int PosicionPrincipalId { get; set; }

        public int? PosicionSecundariaId { get; set; }

        public int? PosicionTerciariaId { get; set; }
    }
}

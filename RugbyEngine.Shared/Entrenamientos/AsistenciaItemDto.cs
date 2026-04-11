namespace RugbyEngine.Shared.Entrenamientos
{
    public class AsistenciaItemDto
    {
        public int JugadorId { get; set; }
        public string JugadorNombre { get; set; } = string.Empty;
        public AsistenciaEstado Estado { get; set; } = AsistenciaEstado.Ausente;
    }
}

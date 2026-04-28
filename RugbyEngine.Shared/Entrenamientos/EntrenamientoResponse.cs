namespace RugbyEngine.Shared.Entrenamientos
{
    public class EntrenamientoResponse
    {
        public int Id { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public DateOnly Fecha { get; set; }
    }
}

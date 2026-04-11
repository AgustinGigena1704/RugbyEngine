namespace RugbyEngine.Shared.Entrenamientos
{
    public class CategoriaResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Abreviatura { get; set; } = string.Empty;
        public int EdadMinima { get; set; }
        public int EdadMaxima { get; set; }
    }
}

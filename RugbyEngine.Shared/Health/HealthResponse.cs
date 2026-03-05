namespace RugbyEngine.Shared.Health
{
    public class HealthResponse
    {
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Service { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
    }
}

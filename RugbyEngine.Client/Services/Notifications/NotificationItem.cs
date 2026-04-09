namespace RugbyEngine.Client.Services.Notifications
{
    public class NotificationItem
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public NotificationType Tipo { get; init; }
        public string Mensaje { get; init; } = string.Empty;
        public int Duracion { get; init; } = 5;
        public string? Detalle { get; init; }
        public Exception? Exception { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}

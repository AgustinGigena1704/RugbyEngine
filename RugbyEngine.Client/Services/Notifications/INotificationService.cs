namespace RugbyEngine.Client.Services.Notifications
{
    public interface INotificationService
    {
        IReadOnlyList<NotificationItem> ActiveNotifications { get; }

        bool ShowDetails { get; }

        event Action? OnChange;

        void Notify(NotificationType tipo, string mensaje, int? duracion = 5, string? detalle = null, Exception? exception = null);

        void Dismiss(Guid id);

        void SetShowDetails(bool value);
    }
}

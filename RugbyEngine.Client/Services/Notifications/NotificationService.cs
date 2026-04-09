namespace RugbyEngine.Client.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly List<NotificationItem> _notifications = [];
        private readonly object _lock = new();

        public IReadOnlyList<NotificationItem> ActiveNotifications
        {
            get
            {
                lock (_lock)
                {
                    return _notifications.ToList().AsReadOnly();
                }
            }
        }

        public bool ShowDetails { get; private set; }

        public event Action? OnChange;

        public void Notify(NotificationType tipo, string mensaje, int? duracion = 5, string? detalle = null, Exception? exception = null)
        {
            var item = new NotificationItem
            {
                Tipo = tipo,
                Mensaje = mensaje,
                Duracion = duracion ?? 5,
                Detalle = detalle,
                Exception = exception
            };

            lock (_lock)
            {
                _notifications.Add(item);

                if (_notifications.Count > 5)
                {
                    _notifications.RemoveAt(0);
                }
            }

            OnChange?.Invoke();
        }

        public void Dismiss(Guid id)
        {
            lock (_lock)
            {
                _notifications.RemoveAll(n => n.Id == id);
            }

            OnChange?.Invoke();
        }

        public void SetShowDetails(bool value)
        {
            if (ShowDetails == value) return;
            ShowDetails = value;
            OnChange?.Invoke();
        }
    }
}

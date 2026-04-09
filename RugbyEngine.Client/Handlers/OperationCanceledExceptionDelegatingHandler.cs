using RugbyEngine.Client.Services.Notifications;
using System.Net;

namespace RugbyEngine.Client.Handlers
{
    public class OperationCanceledExceptionDelegatingHandler : DelegatingHandler
    {
        public static readonly HttpRequestOptionsKey<bool> SuppressNotification = new("SuppressNotification");

        private readonly IServiceProvider _serviceProvider;

        public OperationCanceledExceptionDelegatingHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response;

            try
            {
                response = await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                var ns = _serviceProvider.GetService<INotificationService>();
                ns?.Notify(NotificationType.Error, "No se pudo conectar con el servidor.", detalle: ex.Message, exception: ex);
                throw;
            }

            // Skip notifications if caller opted out
            if (request.Options.TryGetValue(SuppressNotification, out var suppress) && suppress)
                return response;

            // Notify on HTTP error responses (skip 401 to avoid noise on auth redirects)
            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.Unauthorized)
            {
                var ns = _serviceProvider.GetService<INotificationService>();
                if (ns is not null)
                {
                    var detalle = await TryReadErrorBodyAsync(response);
                    var (tipo, mensaje) = response.StatusCode switch
                    {
                        HttpStatusCode.BadRequest => (NotificationType.Warning, "Datos inválidos en la solicitud."),
                        HttpStatusCode.Forbidden => (NotificationType.Warning, "No tiene permisos para esta acción."),
                        HttpStatusCode.NotFound => (NotificationType.Warning, "El recurso solicitado no existe."),
                        HttpStatusCode.Conflict => (NotificationType.Warning, "Conflicto al procesar la solicitud."),
                        HttpStatusCode.InternalServerError => (NotificationType.Error, "Error interno del servidor."),
                        HttpStatusCode.ServiceUnavailable => (NotificationType.Error, "El servidor no está disponible."),
                        _ => (NotificationType.Error, $"Error del servidor ({(int)response.StatusCode}).")
                    };
                    ns.Notify(tipo, mensaje, detalle: detalle);
                }
            }

            return response;
        }

        private static async Task<string?> TryReadErrorBodyAsync(HttpResponseMessage response)
        {
            try
            {
                var body = await response.Content.ReadAsStringAsync();
                return string.IsNullOrWhiteSpace(body) ? null : body;
            }
            catch
            {
                return null;
            }
        }
    }
}

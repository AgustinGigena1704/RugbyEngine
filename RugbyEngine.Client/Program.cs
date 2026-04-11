using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using RugbyEngine.Client;
using RugbyEngine.Client.Handlers;
using RugbyEngine.Client.Services;
using RugbyEngine.Client.Services.Notifications;
using System;

using RugbyEngine.Client.Services.Entrenamientos;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddTransient<OperationCanceledExceptionDelegatingHandler>();

builder.Services.AddScoped(sp =>
{
    var apiBaseUrl = "https://rugbyengine.agigena.com";
    if (builder.HostEnvironment.IsDevelopment())
    {
        apiBaseUrl = "http://localhost:5000";
    }
    var baseAddress = !string.IsNullOrWhiteSpace(apiBaseUrl)
        ? new Uri(apiBaseUrl, UriKind.Absolute)
        : new Uri(builder.HostEnvironment.BaseAddress);

    var handler = ActivatorUtilities.CreateInstance<OperationCanceledExceptionDelegatingHandler>(sp);
    handler.InnerHandler = new HttpClientHandler();

    return new HttpClient(handler) { BaseAddress = baseAddress };
});

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<MainMenuService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPersonaService, PersonaService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IPosicionService, PosicionService>();
builder.Services.AddScoped<IEntrenamientoService, EntrenamientoService>();
builder.Services.AddScoped<IJugadorService, JugadorService>();
builder.Services.AddScoped<IAsistenciaService, AsistenciaService>();

await builder.Build().RunAsync();

using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using RugbyEngine.Client;
using RugbyEngine.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var apiBaseUrl = configuration["API_BASE_URL"] ?? configuration["Api:BaseUrl"] ?? "https://localhost";
    var baseAddress = !string.IsNullOrWhiteSpace(apiBaseUrl)
        ? new Uri(apiBaseUrl, UriKind.Absolute)
        : new Uri(builder.HostEnvironment.BaseAddress);

    return new HttpClient { BaseAddress = baseAddress };
});
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<MainMenuService>();
builder.Services.AddScoped<IPersonaService, PersonaService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddBlazoredSessionStorage();

await builder.Build().RunAsync();

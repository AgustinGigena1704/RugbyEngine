using Microsoft.Extensions.DependencyInjection;
using RugbyEngine.Api.Data.Attributes;
using System.Reflection;

namespace RugbyEngine.Api.Data.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Escanea el assembly de la API y registra automáticamente todas las clases
    /// decoradas con [Service] en el contenedor de DI.
    ///
    /// Agregar en Program.cs:
    ///   builder.Services.AddAnnotatedServices();
    /// </summary>
    public static IServiceCollection AddAnnotatedServices(
        this IServiceCollection services,
        Assembly? assembly = null)
    {
        assembly ??= Assembly.GetExecutingAssembly();

        var candidates = assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false }
                     && t.GetCustomAttribute<ServiceAttribute>() is not null);

        foreach (var type in candidates)
        {
            var attr = type.GetCustomAttribute<ServiceAttribute>()!;
            var serviceType = ResolveServiceType(type, attr);
            var descriptor = new ServiceDescriptor(serviceType, type, attr.Lifetime);

            // Evitar duplicados (útil si AddAnnotatedServices se llama más de una vez)
            if (services.Any(d => d.ServiceType == serviceType && d.ImplementationType == type))
                continue;

            services.Add(descriptor);
        }

        return services;
    }

    // ── Resolución de interfaz ───────────────────────────────

    private static Type ResolveServiceType(Type implementationType, ServiceAttribute attr)
    {
        // 1. Registro concreto explícito
        if (attr.RegisterAsConcrete)
            return implementationType;

        // 2. Interfaz declarada explícitamente en el atributo
        if (attr.InterfaceType is not null)
        {
            AssertImplements(implementationType, attr.InterfaceType);
            return attr.InterfaceType;
        }

        // 3. Convención: I + NombreClase  (ej: JugadorService → IJugadorService)
        var conventionName = $"I{implementationType.Name}";
        var conventionInterface = implementationType
            .GetInterfaces()
            .FirstOrDefault(i => i.Name == conventionName);

        if (conventionInterface is not null)
            return conventionInterface;

        // 4. Fallback: registro concreto si no hay interfaz que coincida
        return implementationType;
    }

    private static void AssertImplements(Type implementation, Type iface)
    {
        if (!iface.IsAssignableFrom(implementation))
            throw new InvalidOperationException(
                $"[Service] en '{implementation.Name}' declara la interfaz '{iface.Name}', " +
                $"pero la clase no la implementa.");
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace RugbyEngine.Api.Data.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ServiceAttribute : Attribute
{
    /// <summary>Lifetime del servicio. Por defecto: Scoped.</summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// Interfaz explícita contra la que se registra.
    /// Si es null se intenta auto-detectar por convención (I + NombreClase).
    /// </summary>
    public Type? InterfaceType { get; }

    /// <summary>
    /// Cuando es true, registra la clase contra sí misma aunque exista una interfaz.
    /// </summary>
    public bool RegisterAsConcrete { get; }

    // ── Constructores ────────────────────────────────────────

    /// <summary>Scoped, interfaz auto-detectada por convención.</summary>
    public ServiceAttribute()
        : this(ServiceLifetime.Scoped) { }

    /// <summary>Lifetime explícito, interfaz auto-detectada por convención.</summary>
    public ServiceAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
        InterfaceType = null;
        RegisterAsConcrete = false;
    }

    /// <summary>Interfaz explícita, lifetime Scoped.</summary>
    public ServiceAttribute(Type interfaceType)
        : this(interfaceType, ServiceLifetime.Scoped) { }

    /// <summary>Interfaz explícita + lifetime.</summary>
    public ServiceAttribute(Type interfaceType, ServiceLifetime lifetime)
    {
        InterfaceType = interfaceType;
        Lifetime = lifetime;
        RegisterAsConcrete = false;
    }

    /// <summary>Registro concreto (sin interfaz), lifetime configurable.</summary>
    public ServiceAttribute(bool registerAsConcrete, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        RegisterAsConcrete = registerAsConcrete;
        Lifetime = lifetime;
        InterfaceType = null;
    }
}

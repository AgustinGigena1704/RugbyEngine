using Microsoft.AspNetCore.Components;

namespace RugbyEngine.Client.Components.Tabla;

public interface ITablaRegistration
{
    Type RowType { get; }
    void ClearDefinition();
    void RegisterHeader(string nombre);
    void RegisterCell(Func<object, RenderFragment> renderer);
}

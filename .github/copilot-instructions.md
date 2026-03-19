# Copilot Instructions

## General Guidelines
- No hardcodear tamaños fijos en tablas; deben ser responsive tanto en mobile como en desktop.
- En la paginación de Personas, usar formato de contador compacto 'x / x2' tanto en mobile como en desktop.
- El backend debe devolver siempre JSON con listas de datos de Personas/Usuarios (sin HTML ni TableResponse).
- Consumir APIs directamente con HttpClient en servicios cliente; evitar el uso de CreateAuthorizedRequestAsync.

---
name: spec-manager
description: Manage specification files for spec-driven development. Use when creating, updating, or deleting spec files. Supports feat, fix, docs, refactor, and chore scopes.
allowed-tools: Read, Write, Glob, Grep, Bash, AskUserQuestion
---

# Spec Manager

Manages specification files in `specs/{scope}/{work-name}.md` for spec-driven development workflow.

## Principles

1. **One spec = one task** (unless frontend + backend work together)
2. **No "before" sections** - Focus only on the final desired state
3. **XML tag format** - Use XML tags for structure (varies by scope)
4. **Minimal code examples** - Avoid code in specs; if necessary, keep it brief and illustrative
5. **Check for duplicates** - Always verify if similar specs exist before creating

## Operations

### Create
1. Analyze request and infer scope (feat/fix/docs/refactor/chore)
2. Search `specs/{scope}/` for similar existing specs
3. If found, ask: "Create new", "Update existing", or "Cancel"
4. If ambiguous scope, ask user to confirm
5. Generate filename (lowercase-with-hyphens)
6. Use appropriate template based on scope
7. Fill in XML tags based on user request

### Update
1. List existing specs in relevant scope
2. Let user select which to update
3. Verify update aligns with original purpose
4. If purpose changed significantly, suggest new spec
5. Apply changes while maintaining XML structure

### Delete
1. List all specs organized by scope
2. Let user select spec(s) to delete
3. Confirm and remove

### List
Display all specs organized by scope directory structure.

## Templates

See templates directory for scope-specific XML tag structures:
- `feat` → [templates/feat-template.md](templates/feat-template.md)
- `fix` → [templates/fix-template.md](templates/fix-template.md)
- `docs` → [templates/docs-template.md](templates/docs-template.md)
- `refactor` → [templates/refactor-template.md](templates/refactor-template.md)
- `chore` → [templates/chore-template.md](templates/chore-template.md)

Each template uses different XML tags appropriate to its scope.

# RugbyEngine Project Instructions

## Spec-Driven Development

Este proyecto usa spec-driven development. Las especificaciones están en `RugbyEngine.Client\specs\{scope}\`.

### Spec Manager Skill

Cuando el usuario mencione "spec" o pida crear/actualizar/listar specs:

1. **Leer**: `RugbyEngine.Client\skills\spec-manager\SKILL.md` para entender el workflow
2. **Scopes**: feat, fix, docs, refactor, chore
3. **Templates**: `RugbyEngine.Client\skills\spec-manager\templates\{scope}-template.md`
4. **Ubicación**: Specs van en `RugbyEngine.Client\specs\{scope}\{nombre}.md`

### Operaciones

- **Crear spec**: Verificar duplicados → Usar template → Generar archivo
- **Actualizar spec**: Listar specs → Seleccionar → Actualizar manteniendo XML
- **Listar specs**: Mostrar estructura `specs/` organizada
- **Eliminar spec**: Listar → Confirmar → Eliminar

### Principios

1. One spec = one task
2. No "before" sections
3. XML tag format
4. Minimal code examples
5. Check for duplicates

# Quick Commands

- "spec create {nombre}": Usa spec-manager para crear nueva spec
- "spec list": Lista todas las specs existentes
- "spec update {nombre}": Actualiza spec existente

Workflow: Siempre lee RugbyEngine.Client/skills/spec-manager/SKILL.md

---

## Usage Instructions

### How to Invoke Spec Manager

**In GitHub Copilot Chat:**

1. **Reference the skill explicitly:**
   ```
   @workspace Usando spec-manager skill, crea una spec para [descripción]
   ```

2. **With file context:**
   ```
   @workspace #file:skills/spec-manager/SKILL.md
   Crea spec para agregar funcionalidad de búsqueda de jugadores
   ```

3. **Direct command pattern:**
   ```
   @workspace spec create password-reset-flow
   ```

### Common Use Cases

#### 1. **Create New Feature Spec**
```
@workspace spec-manager: Crea spec para implementar notificaciones push en tiempo real

# El asistente:
1. Busca duplicados en specs/feat/
2. Usa templates/feat-template.md
3. Te pregunta confirmación si encuentra similar
4. Genera specs/feat/realtime-notifications.md
```

#### 2. **List All Specs**
```
@workspace spec-manager: lista todas las specs

# Output esperado:
specs/
├── feat/
│   ├── jwt-authentication.md
│   ├── mudblazor-layout.md
│   ├── login-page.md
│   ├── api-communication.md
│   ├── route-protection.md
│   └── cookie-token-storage.md
└── docs/
    ├── project-overview.md
    └── ralph-integration.md
```

#### 3. **Update Existing Spec**
```
@workspace spec-manager: actualiza spec jwt-authentication 
para incluir refresh token

# El asistente:
1. Abre specs/feat/jwt-authentication.md
2. Preserva estructura XML
3. Agrega nuevos requirements
4. Mantiene purpose original
```

#### 4. **Create Fix Spec**
```
@workspace spec-manager: crea spec fix para el bug de login 
que no redirige correctamente

# Scope: fix (inferido automáticamente)
# Genera: specs/fix/login-redirect-issue.md
# Template: templates/fix-template.md
```

#### 5. **Create Docs Spec**
```
@workspace spec-manager: documenta el proceso de deployment

# Scope: docs
# Genera: specs/docs/deployment-process.md
```

### Integration with Development Workflow

#### Spec-First Approach:
1. **Define** → Create spec with spec-manager
2. **Review** → Validate spec structure and requirements
3. **Implement** → Code follows spec (manual or with Ralph)
4. **Verify** → Check implementation against spec's `<testing>` section
5. **Update** → Modify spec if implementation deviates

#### Example Full Workflow:
```
# Step 1: Create spec
@workspace spec-manager: crea spec para sistema de permisos por rol

# Step 2: Review generated spec
# Verifica que specs/feat/role-based-permissions.md tenga:
# - <purpose> claro
# - <requirements> completos
# - <implementation> con arquitectura definida
# - <testing> con criterios verificables

# Step 3: Implement
# (Código manualmente o con Ralph)

# Step 4: Verify
dotnet build
# Run tests según <testing> section

# Step 5: Update if needed
@workspace spec-manager: actualiza role-based-permissions 
para reflejar uso de PolicyProvider en lugar de custom middleware
```

### Best Practices

1. **Always check for duplicates** before creating new spec
2. **Use descriptive filenames** (lowercase-with-hyphens)
3. **Keep specs focused** (one spec = one task)
4. **Update specs when implementation changes** significantly
5. **Reference related specs** in `<implementation>` section
6. **Be specific in `<requirements>`** - avoid ambiguity
7. **Make `<testing>` actionable** - clear verification steps

### Scope Selection Guide

| Scope | When to Use | Example |
|-------|-------------|---------|
| `feat` | Nueva funcionalidad | User profile page, Search feature |
| `fix` | Corregir bug | Login redirect bug, Cookie expiration issue |
| `docs` | Documentación | API guide, Architecture docs |
| `refactor` | Mejorar código sin cambiar funcionalidad | Extract service, Optimize queries |
| `chore` | Tareas de mantenimiento | Update dependencies, CI/CD setup |

### Troubleshooting

**Q: Spec manager no detecta mi request**
```
A: Usa palabra clave explícita:
   "@workspace spec-manager: [tu request]"
   o menciona "spec" al inicio
```

**Q: Quiero crear spec pero no sé qué scope usar**
```
A: Describe la tarea, spec-manager inferirá el scope.
   Ejemplo: "@workspace spec-manager: crea spec para corregir 
   el error de timeout en la API"
   → Scope: fix (inferido)
```

**Q: ¿Cómo referencio specs existentes?**
```
A: En <implementation>, menciona:
   "Ver specs/feat/jwt-authentication.md para integración con auth"
```

**Q: ¿Puedo combinar frontend + backend en una spec?**
```
A: Sí, si son parte de la misma feature cohesiva.
   Ejemplo: specs/feat/user-registration.md puede incluir
   tanto el form (client) como el endpoint (API)
```

### Advanced Usage

#### Creating Spec Templates for Custom Patterns
```
# Si tu proyecto tiene patterns específicos,
# crea templates adicionales en:
skills/spec-manager/templates/custom-{pattern}-template.md

# Luego usa:
@workspace spec-manager: crea spec usando template custom-api-integration
```

#### Bulk Operations
```
@workspace spec-manager: lista specs de tipo feat que mencionen "auth"

@workspace spec-manager: actualiza todas las specs para incluir 
prerequisito de .NET 10
```

### Integration with Ralph AI

```
# Workflow completo con Ralph:

1. Crear spec:
   @workspace spec-manager: crea spec para player statistics dashboard

2. Generar código con Ralph:
   @workspace Ralph, implementa specs/feat/player-statistics-dashboard.md
   siguiendo directrices en specs/docs/ralph-integration.md

3. Verificar:
   dotnet build
   # Test manual

4. Actualizar spec si necesario:
   @workspace spec-manager: actualiza player-statistics-dashboard
   para reflejar uso de MudDataGrid en lugar de MudTable
```

---

**Quick Reference Card:**

| Task | Command |
|------|---------|
| Create spec | `@workspace spec-manager: crea spec para {descripción}` |
| List specs | `@workspace spec-manager: lista specs` |
| Update spec | `@workspace spec-manager: actualiza {nombre}` |
| Delete spec | `@workspace spec-manager: elimina {nombre}` |
| View template | `@workspace #file:skills/spec-manager/templates/{scope}-template.md` |

# ══════════════════════════════════════════════════════════════════
#  Estructura requerida del repo (monorepo):
#    repo-root/
#    ├── RugbyEngine/          ← API  (contenido actual del repo)
#    ├── RugbyEngine.Client/   ← Cliente Blazor WASM
#    ├── RugbyEngine.Shared/   ← Modelos compartidos
#    └── Dockerfile            ← este archivo
#
#  Build local:  docker build -t rugbyengine .
#  CI/CD:        ver .github/workflows/deploy.yml
# ══════════════════════════════════════════════════════════════════

# ── Stage 1: Compilar Blazor WASM Client ──────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-client
WORKDIR /src

RUN dotnet workload install wasm-tools

COPY ["RugbyEngine.Shared/RugbyEngine.Shared.csproj",  "RugbyEngine.Shared/"]
COPY ["RugbyEngine.Client/RugbyEngine.Client.csproj",  "RugbyEngine.Client/"]
RUN dotnet restore "RugbyEngine.Client/RugbyEngine.Client.csproj"

COPY RugbyEngine.Shared/ RugbyEngine.Shared/
COPY RugbyEngine.Client/ RugbyEngine.Client/
RUN dotnet publish "RugbyEngine.Client/RugbyEngine.Client.csproj" \
    -c Release -o /app/client

# ── Stage 2: Compilar API ─────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-api
WORKDIR /src

COPY ["RugbyEngine.Shared/RugbyEngine.Shared.csproj", "RugbyEngine.Shared/"]
COPY ["RugbyEngine.Api/RugbyEngine.Api.csproj",       "RugbyEngine.Api/"]
RUN dotnet restore "RugbyEngine.Api/RugbyEngine.Api.csproj"

COPY RugbyEngine.Shared/ RugbyEngine.Shared/
COPY RugbyEngine.Api/ RugbyEngine.Api/
RUN dotnet publish "RugbyEngine.Api/RugbyEngine.Api.csproj" \
    -c Release -o /app/api /p:UseAppHost=false

# ── Stage 3: Imagen final ─────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

# API publicada
COPY --from=build-api /app/api .

# Archivos estáticos del cliente WASM → wwwroot de la API
COPY --from=build-client /app/client/wwwroot ./wwwroot

ENTRYPOINT ["dotnet", "RugbyEngine.Api.dll"]

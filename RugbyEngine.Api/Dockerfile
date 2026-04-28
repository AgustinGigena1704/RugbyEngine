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

RUN apt-get update && apt-get install -y --no-install-recommends python3 && rm -rf /var/lib/apt/lists/*

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

# Ensure dotnet-ef is available during SDK build stages (useful for running migrations inside the container in CI/dev)
RUN dotnet tool install --global dotnet-ef --version 10.0.5 || true
ENV PATH="$PATH:/root/.dotnet/tools"

COPY RugbyEngine.Shared/ RugbyEngine.Shared/
COPY RugbyEngine.Api/ RugbyEngine.Api/
COPY [".devcontainer/bin/setup.sh", "/src/setup.sh"]
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

# copiar setup.sh desde la etapa build-api para que esté disponible en la imagen final
COPY --from=build-api /src/setup.sh /app/setup.sh
RUN chmod +x /app/setup.sh || true

ENTRYPOINT ["dotnet", "RugbyEngine.Api.dll"]

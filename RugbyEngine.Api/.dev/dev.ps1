# ============================================================
#  RugbyEngine - Dev CLI
#  Uso: .\dev.ps1 db <setup|update|down>
# ============================================================

param(
    [Parameter(Position = 0)] [string]$Command    = "",
    [Parameter(Position = 1)] [string]$SubCommand = ""
)

$ErrorActionPreference = "Continue"

$ScriptDir       = Split-Path -Parent $MyInvocation.MyCommand.Path
$ComposeFile     = Join-Path $ScriptDir "docker\docker-compose.yml"
$ContainerName   = "rugbyengine_dev_db"
$VolumeName      = "rugbyengine_dev_data"
$DevDbName       = "rugbyengine_dev"
$DevDbUser       = "dev_user"
$DevDbPass       = "dev_password"
$DevDbPort       = "5433"
$TempDump        = Join-Path $ScriptDir "temp_dump.sql"

# ── Colores ─────────────────────────────────────────────────

function Write-Header([string]$msg) {
    Write-Host ""
    Write-Host "══════════════════════════════════════════" -ForegroundColor DarkCyan
    Write-Host "  $msg" -ForegroundColor Cyan
    Write-Host "══════════════════════════════════════════" -ForegroundColor DarkCyan
}

function Write-Ok([string]$msg)   { Write-Host "  ✔  $msg" -ForegroundColor Green  }
function Write-Warn([string]$msg) { Write-Host "  ⚠  $msg" -ForegroundColor Yellow }
function Write-Err([string]$msg)  { Write-Host "  ✘  $msg" -ForegroundColor Red    }
function Write-Info([string]$msg) { Write-Host "  →  $msg" -ForegroundColor Gray   }

# ── Credenciales de producción ───────────────────────────────

function Get-ProdCredentials {
    Write-Header "Credenciales de producción"
    Write-Warn "Los datos se usan únicamente para el dump y NO se persisten."
    Write-Host ""

    $creds = @{}

    $creds.Host   = Read-Host "  [1/6] HOST"
    $creds.Port   = Read-Host "  [2/6] PORT"
    $creds.DB     = Read-Host "  [3/6] DB"
    $creds.User   = Read-Host "  [4/6] USER"

    $secPass      = Read-Host "  [5/6] PASS" -AsSecureString
    $creds.Pass   = [System.Net.NetworkCredential]::new("", $secPass).Password

    $creds.CAPath = Read-Host "  [6/6] CA_PATH (ruta absoluta al certificado .crt)"

    # Validar CA
    if (-not (Test-Path $creds.CAPath)) {
        Write-Err "El archivo de certificado no existe: $($creds.CAPath)"
        exit 1
    }

    return $creds
}

# ── Docker helpers ────────────────────────────────────────────

function Start-DevContainer {
    Write-Info "Levantando contenedor PostgreSQL..."

    docker compose -f $ComposeFile up -d 2>&1 | ForEach-Object {
        $line = $_.ToString()
        if ($line -match "error|Error|ERROR") {
            Write-Err $line
        } else {
            Write-Info $line
        }
    }
    $exitCode = $LASTEXITCODE

    if ($exitCode -ne 0) {
        Write-Err "docker compose up falló (exit code $exitCode)."
        exit 1
    }

    Write-Info "Esperando que PostgreSQL esté healthy..."
    $attempts = 0
    do {
        Start-Sleep -Seconds 3
        $status = docker inspect --format="{{.State.Health.Status}}" $ContainerName 2>$null
        $attempts++
        Write-Host "    ($attempts/20) estado: $status" -ForegroundColor DarkGray
    } while ($status -ne "healthy" -and $attempts -lt 20)

    if ($status -ne "healthy") {
        Write-Err "El contenedor no respondió en tiempo. Revisá los logs: docker logs $ContainerName"
        exit 1
    }

    Write-Ok "Contenedor listo."
}

function Stop-DevContainer {
    Write-Info "Deteniendo contenedor (el volumen se conserva)..."
    docker compose -f $ComposeFile stop 2>&1 | ForEach-Object { Write-Info $_ }
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Error al detener el contenedor."
        exit 1
    }
    Write-Ok "Contenedor detenido. Usá 'db setup' para retomar."
}

function Test-ContainerRunning {
    $name = docker ps --filter "name=^/${ContainerName}$" --format "{{.Names}}" 2>$null
    return ($name -eq $ContainerName)
}

function Test-VolumeExists {
    $vol = docker volume ls --filter "name=^${VolumeName}$" --format "{{.Name}}" 2>$null
    return ($vol -eq $VolumeName)
}

function Test-DevDbHasData {
    $count = docker exec $ContainerName psql -U $DevDbUser -d $DevDbName -tAc `
        "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public';" 2>$null
    return ($count -match '^\s*[1-9]\d*\s*$')
}

# ── pg_dump desde producción (via contenedor efímero) ────────

function Invoke-ProdDump([hashtable]$creds) {
    Write-Header "Descargando dump desde producción"

    $caFileName = Split-Path $creds.CAPath -Leaf
    $caDir      = (Resolve-Path (Split-Path $creds.CAPath -Parent)).Path.TrimEnd('\').TrimEnd('/')
    $caDir      = $caDir -replace '\\', '/'

    # Limpiar dump previo
    if (Test-Path $TempDump) { Remove-Item $TempDump -Force }

    $encUser = [Uri]::EscapeDataString($creds.User)
    $encPass = [Uri]::EscapeDataString($creds.Pass)
    $connStr = "postgresql://${encUser}:${encPass}@$($creds.Host):$($creds.Port)/$($creds.DB)?sslmode=verify-ca&sslrootcert=/certs/${caFileName}"

    Write-Info "Ejecutando pg_dump (puede tardar según el tamaño de la BD)..."

    # postgres:17-alpine para coincidir con la versión del servidor de producción (PG 17)
    docker run --rm `
        -v "${caDir}:/certs:ro" `
        postgres:17-alpine `
        pg_dump --no-owner --no-acl --clean --if-exists `
        $connStr `
        | Out-File -FilePath $TempDump -Encoding UTF8 -Force

    if ($LASTEXITCODE -ne 0) {
        Write-Err "pg_dump falló. Verificá credenciales, conectividad y el certificado."
        if (Test-Path $TempDump) { Remove-Item $TempDump -Force }
        exit 1
    }

    $size = [math]::Round((Get-Item $TempDump).Length / 1MB, 2)
    Write-Ok "Dump generado: $TempDump ($size MB)"
}

# ── Restaurar dump al contenedor ─────────────────────────────

function Invoke-Restore {
    Write-Header "Restaurando base de datos en el contenedor"

    Write-Info "Dropeando y recreando '$DevDbName'..."
    docker exec $ContainerName psql -U $DevDbUser -d postgres -c `
        "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '$DevDbName' AND pid <> pg_backend_pid();" | Out-Null
    docker exec $ContainerName psql -U $DevDbUser -d postgres -c `
        "DROP DATABASE IF EXISTS $DevDbName;" | Out-Null
    docker exec $ContainerName psql -U $DevDbUser -d postgres -c `
        "CREATE DATABASE $DevDbName;" | Out-Null

    Write-Info "Copiando dump al contenedor..."
    docker cp $TempDump "${ContainerName}:/tmp/restore.sql"

    Write-Info "Aplicando dump..."
    docker exec $ContainerName psql -U $DevDbUser -d $DevDbName -f /tmp/restore.sql -q

    if ($LASTEXITCODE -ne 0) {
        Write-Warn "psql reportó warnings durante la restauración (puede ser normal con --clean)."
    }

    docker exec $ContainerName rm /tmp/restore.sql
    Remove-Item $TempDump -Force

    Write-Ok "Base de datos restaurada correctamente."
    Write-Host ""
    Write-Host "  Cadena de conexión local:" -ForegroundColor DarkCyan
    Write-Host "  Server=localhost;Port=${DevDbPort};Database=${DevDbName};User Id=${DevDbUser};Password=${DevDbPass};" -ForegroundColor Cyan
}

# ── Comandos ─────────────────────────────────────────────────

switch ($Command.ToLower()) {

    "db" {
        switch ($SubCommand.ToLower()) {

            # ── db setup ──────────────────────────────────────
            "setup" {
                Write-Header "DB SETUP"

                if (Test-ContainerRunning) {
                    Write-Ok "El contenedor ya está corriendo."

                    if (Test-DevDbHasData) {
                        Write-Ok "La base de datos ya tiene datos. No se necesita restaurar."
                        Write-Host ""
                        Write-Host "  postgresql://${DevDbUser}:${DevDbPass}@localhost:${DevDbPort}/${DevDbName}" -ForegroundColor Cyan
                        exit 0
                    }

                    Write-Warn "El contenedor está corriendo pero la DB está vacía. Se restaurará desde producción."
                }
                else {
                    if (Test-VolumeExists) {
                        Write-Info "Volumen '$VolumeName' encontrado. Levantando contenedor..."
                        Start-DevContainer

                        if (Test-DevDbHasData) {
                            Write-Ok "Datos existentes detectados en el volumen. ¡Listo para desarrollar!"
                            Write-Host ""
                            Write-Host "  postgresql://${DevDbUser}:${DevDbPass}@localhost:${DevDbPort}/${DevDbName}" -ForegroundColor Cyan
                            exit 0
                        }

                        Write-Warn "El volumen existe pero la DB está vacía. Se restaurará desde producción."
                    }
                    else {
                        Write-Info "Primer uso detectado. Levantando contenedor nuevo..."
                        Start-DevContainer
                    }
                }

                $creds = Get-ProdCredentials
                Invoke-ProdDump $creds
                Invoke-Restore
            }

            # ── db update ─────────────────────────────────────
            "update" {
                Write-Header "DB UPDATE"

                if (-not (Test-ContainerRunning)) {
                    Write-Info "El contenedor no está corriendo. Levantando..."
                    Start-DevContainer
                }

                Write-Warn "Se dropeará la base de datos local y se reemplazará con producción."
                $confirm = Read-Host "  ¿Confirmar? (s/N)"
                if ($confirm -notmatch '^[sS]$') {
                    Write-Info "Operación cancelada."
                    exit 0
                }

                $creds = Get-ProdCredentials
                Invoke-ProdDump $creds
                Invoke-Restore
            }

            # ── db down ───────────────────────────────────────
            "down" {
                Write-Header "DB DOWN"

                if (-not (Test-ContainerRunning)) {
                    Write-Warn "El contenedor ya está detenido."
                    exit 0
                }

                Stop-DevContainer
            }

            default {
                Write-Warn "Subcomando desconocido: '$SubCommand'"
                Write-Host ""
                Write-Host "  Subcomandos disponibles:  setup | update | down" -ForegroundColor Yellow
                exit 1
            }
        }
    }

    default {
        Write-Host @"

  ██████╗ ██╗   ██╗ ██████╗ ██████╗ ██╗   ██╗
  ██╔══██╗██║   ██║██╔════╝ ██╔══██╗╚██╗ ██╔╝
  ██████╔╝██║   ██║██║  ███╗██████╔╝ ╚████╔╝
  ██╔══██╗██║   ██║██║   ██║██╔══██╗  ╚██╔╝
  ██║  ██║╚██████╔╝╚██████╔╝██████╔╝   ██║
  ╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═════╝   ╚═╝  Dev CLI

  Uso: .\dev.ps1 db <subcomando>

  Subcomandos:
    setup    Levanta el contenedor. Si el volumen tiene datos, lo usa directo.
             Si no, pide credenciales de producción y restaura la base.

    update   Dropea la DB del contenedor, pide credenciales y actualiza desde prod.

    down     Apaga el contenedor conservando el volumen para retomar más tarde.

  Ejemplos:
    .\dev.ps1 db setup
    .\dev.ps1 db update
    .\dev.ps1 db down

"@ -ForegroundColor Cyan
    }
}
# ──────────────────────────────────────────────────────
#  Wrapper raíz — delega todo al script en .dev/
#  Llamar desde la raíz del proyecto: .\dev db setup
# ──────────────────────────────────────────────────────
& "$PSScriptRoot\.dev\dev.ps1" @args
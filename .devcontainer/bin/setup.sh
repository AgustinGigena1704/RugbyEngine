#!/usr/bin/env bash
set -euo pipefail

echo "[setup.sh] Starting setup script"

# Update apt and install required packages
echo "[setup.sh] Installing system packages..."
sudo apt-get update -y
sudo apt-get install -y postgresql-client

# Install helper binary if present
if [ -f ".devcontainer/bin/gcr" ]; then
  echo "[setup.sh] Installing gcr helper..."
  sudo install -m 755 .devcontainer/bin/gcr /usr/local/bin/gcr
fi

echo "[setup.sh] Restoring .NET solution and local tools..."
dotnet restore RugbyEngine.slnx
dotnet tool restore

# If dotnet-ef isn't present in the expected tools folder, try installing it globally
if [ ! -f "$HOME/.dotnet/tools/dotnet-ef" ]; then
  echo "[setup.sh] dotnet-ef binary not found in $HOME/.dotnet/tools; attempting global install..."
  dotnet tool install --global dotnet-ef --version 10.0.5 || true
fi

# Ensure dotnet local tools path is available for all shells
DOTNET_TOOLS_PATH="$HOME/.dotnet/tools"

# Create a profile.d script that uses $HOME so it works for the active user at login
sudo tee /etc/profile.d/dotnet-tools.sh > /dev/null <<'EOF' || true
export PATH="$PATH:$HOME/.dotnet/tools"
EOF

# Ensure PATH addition for common interactive shells (zsh, bash) and login shells (.profile)
for rc in "$HOME/.zshrc" "$HOME/.bashrc" "$HOME/.profile"; do
  if [ -f "$rc" ]; then
    if ! grep -q 'export PATH="\$PATH:\$HOME/.dotnet/tools"' "$rc" 2>/dev/null; then
      echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> "$rc" || true
    fi
  else
    echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> "$rc" || true
  fi
done

echo "[setup.sh] Ensuring script is executable"
chmod +x .devcontainer/bin/setup.sh || true

echo "[setup.sh] Checking dotnet-ef availability..."
if command -v dotnet-ef >/dev/null 2>&1; then
  echo "[setup.sh] dotnet-ef is available: $(dotnet-ef --version || true)"
else
  # Create a symlink in /usr/local/bin pointing to the user's dotnet tools binary so it's immediately runnable
  if [ -f "$HOME/.dotnet/tools/dotnet-ef" ]; then
    echo "[setup.sh] Creating symlink /usr/local/bin/dotnet-ef -> $HOME/.dotnet/tools/dotnet-ef"
    sudo ln -sf "$HOME/.dotnet/tools/dotnet-ef" /usr/local/bin/dotnet-ef || true
    sudo chmod +x /usr/local/bin/dotnet-ef || true
    if command -v dotnet-ef >/dev/null 2>&1; then
      echo "[setup.sh] dotnet-ef is now available: $(dotnet-ef --version || true)"
    fi
  fi

  if ! command -v dotnet-ef >/dev/null 2>&1; then
    echo "[setup.sh] dotnet-ef not in PATH; trying via 'dotnet tool run'"
    if dotnet tool run dotnet-ef -- --version >/dev/null 2>&1; then
      echo "[setup.sh] dotnet-ef is available via 'dotnet tool run dotnet-ef'"
    else
      echo "[setup.sh] dotnet-ef not found. You can install globally with:"
      echo "  dotnet tool install --global dotnet-ef --version 10.0.5"
    fi
  fi
fi

echo "[setup.sh] Done. You may need to open a new terminal or run 'source ~/.zshrc'."

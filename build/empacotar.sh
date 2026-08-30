#!/usr/bin/env bash
# Empacota os releases instalaveis, UM ZIP POR PACOTE.
#
#   build/empacotar.sh            # todos os pacotes
#   build/empacotar.sh core       # so o Core
#   build/empacotar.sh alterrahub # so o Alterra Hub
#
# NAO instala nada: so escreve em dist/. Instalar e decisao de quem baixa.
set -euo pipefail

RAIZ="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$RAIZ"

: "${NautilusDll:=$RAIZ/refs/Nautilus.dll}"
[ -f "$NautilusDll" ] || { echo "ERRO: Nautilus.dll nao encontrado em $NautilusDll (ver refs/README.md)"; exit 1; }

# Cada pacote e uma pasta propria em BepInEx/plugins/, com sua versao e seu ZIP.
# É o que permite lancar um mod sem esperar o outro.
empacotar() {
  local nome="$1" csproj="$2" versao="$3" leiame="$4"; shift 4
  local dlls=("$@")

  local pkg="dist/$nome-v$versao"
  local plugins="$pkg/BepInEx/plugins/$nome"

  echo "→ $nome v$versao"
  dotnet build "$csproj" -c Release -p:NautilusDll="$NautilusDll" >/dev/null

  rm -rf "$pkg" "$pkg.zip"
  mkdir -p "$plugins"
  for d in "${dlls[@]}"; do cp "$d" "$plugins/"; done
  cp LICENSE CREDITOS.md "$pkg/"
  cp "$leiame" "$pkg/LEIA-ME.md"
  [ -f "src/mods/$nome/LICENSE-FCS.txt" ] && cp "src/mods/$nome/LICENSE-FCS.txt" "$pkg/"

  # O .gitignore ja barra binario no repo, mas o pacote e o que sai para fora:
  # vale conferir aqui tambem que nenhum DLL de terceiro entrou por engano.
  local intrusos
  intrusos=$(find "$pkg" -name '*.dll' ! -name 'Unhinged.*.dll' -print)
  [ -z "$intrusos" ] || { echo "ERRO: DLL de terceiro no pacote:"; echo "$intrusos"; exit 1; }

  ( cd dist && zip -r -q "$nome-v$versao.zip" "$nome-v$versao" )
  echo "   $pkg.zip"
  sha256sum "$pkg.zip"
}

ALVO="${1:-todos}"
mkdir -p dist

if [ "$ALVO" = "todos" ] || [ "$ALVO" = "core" ]; then
  V=$(grep -oP '(?<=Version = ")[^"]+' src/Unhinged.Core/UnhingedInfo.cs)
  empacotar "SubnauticaUnhinged" "Unhinged.sln" "$V" "docs/LEIA-ME-RELEASE.md" \
    artifacts/Unhinged.Core/Release/Unhinged.Core.dll \
    artifacts/Unhinged.Legacy/Release/Unhinged.Legacy.dll
fi

if [ "$ALVO" = "todos" ] || [ "$ALVO" = "alterrahub" ]; then
  V=$(grep -oP '(?<=<Version>)[^<]+' src/mods/AlterraHub/AlterraHub.csproj)
  empacotar "AlterraHub" "src/mods/AlterraHub/AlterraHub.csproj" "$V" "src/mods/AlterraHub/LEIA-ME-RELEASE.md" \
    artifacts/AlterraHub/Release/Unhinged.AlterraHub.dll \
    artifacts/AlterraHub/Release/Unhinged.Legacy.dll
fi

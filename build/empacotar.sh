#!/usr/bin/env bash
# Empacota o release instalável. Uso: build/empacotar.sh [versao]
#
# Gera dist/SubnauticaUnhinged-vX.Y.Z.zip com a árvore que se mescla na pasta do jogo.
# NÃO instala nada: só escreve em dist/. Instalar é decisão de quem baixa.
set -euo pipefail

RAIZ="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$RAIZ"

VERSAO="${1:-$(grep -oP '(?<=Version = ")[^"]+' src/Unhinged.Core/UnhingedInfo.cs)}"
PKG="dist/SubnauticaUnhinged-v${VERSAO}"
PLUGINS="$PKG/BepInEx/plugins/SubnauticaUnhinged"

: "${NautilusDll:=$RAIZ/refs/Nautilus.dll}"
[ -f "$NautilusDll" ] || { echo "ERRO: Nautilus.dll não encontrado em $NautilusDll (ver refs/README.md)"; exit 1; }

echo "→ compilando Release…"
rm -rf artifacts
dotnet build Unhinged.sln -c Release -p:NautilusDll="$NautilusDll"

echo "→ montando $PKG…"
rm -rf "$PKG" "$PKG.zip"
mkdir -p "$PLUGINS"
cp artifacts/Unhinged.Core/Release/Unhinged.Core.dll     "$PLUGINS/"
cp artifacts/Unhinged.Legacy/Release/Unhinged.Legacy.dll "$PLUGINS/"
cp LICENSE CREDITOS.md "$PKG/"
cp docs/LEIA-ME-RELEASE.md "$PKG/LEIA-ME.md"

# Rede de segurança: o .gitignore já barra binário no repo, mas o pacote é o que sai
# para fora — vale conferir aqui também que nenhum DLL de terceiro entrou por engano.
INTRUSOS=$(find "$PKG" -name '*.dll' ! -name 'Unhinged.*.dll' -print)
[ -z "$INTRUSOS" ] || { echo "ERRO: DLL de terceiro no pacote:"; echo "$INTRUSOS"; exit 1; }

echo "→ zipando…"
( cd dist && zip -r -q "SubnauticaUnhinged-v${VERSAO}.zip" "SubnauticaUnhinged-v${VERSAO}" )

echo
echo "pronto: $PKG.zip"
sha256sum "$PKG.zip"

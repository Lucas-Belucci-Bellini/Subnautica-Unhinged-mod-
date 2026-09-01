#!/usr/bin/env bash
# Confere que todo namespace ligavel/desligavel no Plugin.cs tem mesmo um
# [QModCore] no assembly compilado.
#
# Existe porque um interruptor que nao casa com namespace nenhum e PIOR que
# interruptor nenhum: ele aparece no .cfg, o jogador desliga, e o modulo carrega
# assim mesmo. O namespace do Cyclops (`CyclopsUpgradeConsole`, sem o `FCS_` da
# pasta) ja caiu nessa.
#
# A lista sai do PROPRIO Plugin.cs — uma fonte so, sem duplicar em lugar nenhum.
set -euo pipefail
RAIZ="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$RAIZ"

NS=$(grep -oP '^\s*\("\K[A-Za-z_][\w.]*(?=",\s*"Enable)' src/mods/AlterraHub/Plugin.cs | paste -sd, -)
[ -n "$NS" ] || { echo "ERRO: nao consegui extrair os namespaces do Plugin.cs."; exit 1; }

echo "modulos declarados no Plugin.cs: $(echo "$NS" | tr ',' '\n' | wc -l)"
MODULOS="$NS" ./tools/VerificarPatches/rodar.sh

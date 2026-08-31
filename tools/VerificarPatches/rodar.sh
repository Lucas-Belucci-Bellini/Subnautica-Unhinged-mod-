#!/usr/bin/env bash
# Confere os alvos de [HarmonyPatch] contra as assemblies reais do jogo.
#
#   tools/VerificarPatches/rodar.sh [caminho-do-dll]
#   MEMBROS="Tipo::Membro,..." tools/VerificarPatches/rodar.sh   # alvos imperativos
set -euo pipefail
RAIZ="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$RAIZ"

# ⚠️ NAO fixar /root/.nuget: a pasta do cache muda com o usuario. No runner do
# GitHub ela e /home/runner/.nuget e /root nem e legivel — foi assim que este
# script quebrou no CI passando limpo aqui. Perguntar ao proprio dotnet.
P="${NUGET_PACKAGES:-}"
if [ -z "$P" ]; then
  P=$(dotnet nuget locals global-packages --list 2>/dev/null \
      | sed -n 's/^global-packages: *//p' | head -1)
fi
[ -n "$P" ] || P="$HOME/.nuget/packages"
[ -d "$P" ] || { echo "ERRO: cache do NuGet nao encontrado (tentei '$P')."; exit 1; }

ALVO="${1:-artifacts/AlterraHub/Release/Unhinged.AlterraHub.dll}"
[ -f "$ALVO" ] || { echo "ERRO: '$ALVO' nao existe — compile o pacote antes."; exit 1; }

dotnet artifacts/VerificarPatches/Release/VerificarPatches.dll "$ALVO" \
  "$P/subnautica.gamelibs/82304.0.0-r.0/lib/net472" \
  "$P/unityengine.modules/2019.4.36/lib/net45" \
  "$P/harmonyx/2.7.0/lib/net45" \
  "$P/bepinex.baselib/5.4.20/lib/net35" \
  "$P/newtonsoft.json/12.0.3/lib/net45" \
  "$P/microsoft.netframework.referenceassemblies.net472/1.0.3/build/.NETFramework/v4.7.2" \
  refs artifacts/AlterraHub/Release

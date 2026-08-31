#!/usr/bin/env bash
# Confere os alvos de [HarmonyPatch] contra as assemblies reais do jogo.
#   tools/VerificarPatches/rodar.sh [caminho-do-dll]
set -euo pipefail
RAIZ="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$RAIZ"
P=/root/.nuget/packages
ALVO="${1:-artifacts/AlterraHub/Release/Unhinged.AlterraHub.dll}"
dotnet artifacts/VerificarPatches/Release/VerificarPatches.dll "$ALVO" \
  "$P/subnautica.gamelibs/82304.0.0-r.0/lib/net472" \
  "$P/unityengine.modules/2019.4.36/lib/net45" \
  "$P/harmonyx/2.7.0/lib/net45" \
  "$P/bepinex.baselib/5.4.20/lib/net35" \
  "$P/newtonsoft.json/12.0.3/lib/net45" \
  "$P/microsoft.netframework.referenceassemblies.net472/1.0.3/build/.NETFramework/v4.7.2" \
  refs artifacts/AlterraHub/Release

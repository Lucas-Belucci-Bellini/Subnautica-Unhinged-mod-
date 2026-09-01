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

# Metadados por pacote para o BUILD-MANIFEST. Um mod incorporado carrega a
# proveniencia do upstream dele; o que e codigo nosso diz isso, em vez de fingir
# um upstream que nao existe.
manifesto_mod() {
  case "$1" in
    AlterraHub)  echo "FC Studios" ;;
    ScannerRoom) echo "(codigo proprio do Unhinged)" ;;
    *)           echo "(codigo proprio do Unhinged)" ;;
  esac
}
manifesto_tag() {
  case "$1" in
    AlterraHub)  echo "fcs-v$2" ;;
    ScannerRoom) echo "scannerroom-v$2" ;;
    *)           echo "core-v$2" ;;
  esac
}
manifesto_upstream() {
  case "$1" in
    AlterraHub) echo "https://github.com/ccgould/FCStudios_SubnauticaMods" ;;
    *)          echo "(nao aplicavel)" ;;
  esac
}
manifesto_branch() {
  case "$1" in AlterraHub) echo "master" ;; *) echo "(nao aplicavel)" ;; esac
}
# ⚠️ NAO ler do checkout atual. A branch de modernizacao e um fato historico do
# mod — onde o porte foi feito —, e o build da release roda a partir do `main`,
# ja depois do merge. Ler `git rev-parse --abbrev-ref HEAD` gravava "main" e
# apagava justamente a informacao que o campo existe para preservar.
manifesto_branch_mod() {
  case "$1" in
    AlterraHub) echo "feature/fcs-modernization" ;;
    *)          echo "(sem branch dedicada)" ;;
  esac
}
manifesto_commit() {
  case "$1" in
    AlterraHub) echo "4275d847de6e0f24c711b4b2a9f4308c10ea8248" ;;
    *)          echo "(nao aplicavel)" ;;
  esac
}

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

  # BUILD-MANIFEST: responde "de onde veio este ZIP?" meses depois, sem depender
  # da memoria de ninguem. Vai DENTRO do pacote e tambem sai solto como asset.
  {
    echo "Project:              Subnautica Unhinged"
    echo "Integrated Mod:       $(manifesto_mod "$nome")"
    echo "Release:              $versao"
    echo "Tag:                  $(manifesto_tag "$nome" "$versao")"
    echo "Source Repository:    $(manifesto_upstream "$nome")"
    echo "Source Branch:        $(manifesto_branch "$nome")"
    echo "Source Commit:        $(manifesto_commit "$nome")"
    echo "Modernization Branch: $(manifesto_branch_mod "$nome")"
    echo "Integrated Commit:    $(git rev-parse HEAD 2>/dev/null || echo '?')"
    echo "Subnautica Build:     82304"
    echo "BepInEx:              5.4.21"
    echo "Nautilus:             1.0.0-pre.53"
    echo "Build Configuration:  Release"
    echo "Build Date:           $(date -u '+%Y-%m-%dT%H:%M:%SZ')"
    echo "Verification Level:   Build verified (NAO testado em jogo)"
  } > "$pkg/BUILD-MANIFEST.txt"
  cp "$leiame" "$pkg/LEIA-ME.md"
  [ -f "src/mods/$nome/LICENSE-FCS.txt" ] && cp "src/mods/$nome/LICENSE-FCS.txt" "$pkg/"

  # O .gitignore ja barra binario no repo, mas o pacote e o que sai para fora:
  # vale conferir aqui tambem que nenhum DLL de terceiro entrou por engano.
  local intrusos
  intrusos=$(find "$pkg" -name '*.dll' ! -name 'Unhinged.*.dll' -print)
  [ -z "$intrusos" ] || { echo "ERRO: DLL de terceiro no pacote:"; echo "$intrusos"; exit 1; }

  # ⚠️ O ZIP e compactado DE DENTRO de $pkg, e nao de dist/ com a pasta como
  # argumento. A diferenca decide se o mod carrega:
  #
  #   errado:  AlterraHub-v1.0.6/BepInEx/plugins/...   (raiz = a pasta do pacote)
  #   certo:   BepInEx/plugins/...                     (raiz = BepInEx)
  #
  # Quem extrai na pasta do Subnautica com o layout errado termina com
  # `Subnautica/AlterraHub-v1.0.6/BepInEx/plugins/`, que o BepInEx nunca varre —
  # o jogo abre normal, nenhum erro aparece, e o mod simplesmente nao existe.
  # Nem uma linha no LogOutput.log, porque nem o chainloader chega a ve-lo.
  # Foi exatamente esse o defeito da v1.0.6. O Vortex tambem so reconhece o
  # pacote com BepInEx/ na raiz.
  ( cd "$pkg" && zip -r -q "$RAIZ/$pkg.zip" . )

  # Conferir o artefato, e nao a intencao: se a raiz do ZIP nao for BepInEx/,
  # o pacote esta quebrado do mesmo jeito que a v1.0.6 estava.
  unzip -l "$pkg.zip" | grep -q ' BepInEx/plugins/' \
    || { echo "ERRO: $pkg.zip nao tem BepInEx/plugins/ na raiz."; exit 1; }
  ! unzip -l "$pkg.zip" | grep -qE " $nome-v$versao/" \
    || { echo "ERRO: $pkg.zip embrulhou tudo numa pasta de topo."; exit 1; }

  # O manifesto tambem sai SOLTO, para poder ir como asset da release sem que
  # ninguem precise abrir o ZIP para saber de onde ele veio.
  cp "$pkg/BUILD-MANIFEST.txt" "dist/BUILD-MANIFEST-$nome.txt"
  GERADOS+=("$nome-v$versao.zip")

  echo "   $pkg.zip"
  sha256sum "$pkg.zip"
}

ALVO="${1:-todos}"
mkdir -p dist
# So os ZIPs desta execucao entram no SHA256SUMS. `dist/` pode ter sobra de
# build anterior, e um checksum que lista artefato de outra build engana quem
# for conferir.
GERADOS=()

if [ "$ALVO" = "todos" ] || [ "$ALVO" = "core" ]; then
  V=$(grep -oP '(?<=Version = ")[^"]+' src/Unhinged.Core/UnhingedInfo.cs)
  # SO o Core: ele NAO referencia o Unhinged.Legacy (confirmado no DLL compilado —
  # so 0Harmony e BepInEx). Enviar a ponte aqui tambem colocava DUAS copias do mesmo
  # assembly em pastas diferentes de plugins/ para quem instalasse Core + AlterraHub,
  # que e receita de conflito de identidade de assembly. A ponte vai so onde e usada.
  empacotar "SubnauticaUnhinged" "Unhinged.sln" "$V" "docs/LEIA-ME-RELEASE.md" \
    artifacts/Unhinged.Core/Release/Unhinged.Core.dll
fi

if [ "$ALVO" = "todos" ] || [ "$ALVO" = "scannerroom" ]; then
  V=$(grep -oP '(?<=<Version>)[^<]+' src/mods/ScannerRoom/ScannerRoom.csproj)
  empacotar "ScannerRoom" "src/mods/ScannerRoom/ScannerRoom.csproj" "$V" "src/mods/ScannerRoom/LEIA-ME-RELEASE.md" \
    artifacts/ScannerRoom/Release/Unhinged.ScannerRoom.dll
fi

if [ "$ALVO" = "todos" ] || [ "$ALVO" = "alterrahub" ]; then
  V=$(grep -oP '(?<=<Version>)[^<]+' src/mods/AlterraHub/AlterraHub.csproj)
  empacotar "AlterraHub" "src/mods/AlterraHub/AlterraHub.csproj" "$V" "src/mods/AlterraHub/LEIA-ME-RELEASE.md" \
    artifacts/AlterraHub/Release/Unhinged.AlterraHub.dll \
    artifacts/AlterraHub/Release/Unhinged.Legacy.dll
fi

# SHA256SUMS: arquivo proprio, no formato que `sha256sum -c` le de volta. Sai
# como asset da release — checksum no meio de um texto ninguem confere.
if [ ${#GERADOS[@]} -eq 0 ]; then
  echo "nenhum pacote gerado — SHA256SUMS nao escrito"
else
  ( cd dist && sha256sum "${GERADOS[@]}" > SHA256SUMS )
  echo
  echo "→ dist/SHA256SUMS (${#GERADOS[@]} pacote(s) desta execucao)"
  sed 's/^/   /' dist/SHA256SUMS
fi

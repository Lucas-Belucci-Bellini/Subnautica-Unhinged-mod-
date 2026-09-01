# Migração de assets do FCStudios

## A resposta curta

**Nenhum asset foi migrado, porque não há nenhum para migrar.**

O repositório upstream (`ccgould/FCStudios_SubnauticaMods`, commit `4275d84`)
**não versiona um único asset de jogo**. Auditoria por extensão de arquivo, no
repositório inteiro:

| tipo | quantidade | o que é |
| --- | ---: | --- |
| `.dll` | 19 | bibliotecas de framework/terceiros (§2) |
| `.json` | 7 | configuração de projeto |
| `.xml` | 6 | configuração de projeto |
| `.targets` | 4 | MSBuild |
| `.md`, `.txt` | 3 | documentação e a licença |
| `.exe` | 1 | `NStrip.exe`, ferramenta de publicização — não usamos |
| **imagem, áudio, modelo, `.assets`, AssetBundle** | **0** | — |

Os modelos, ícones, telas e sons vivem em **asset bundles distribuídos fora do
repositório** (Nexus Mods). Não estão no código-fonte, então não passam por este
porte — e não podem ser redistribuídos por nós.

## 1. O que o código espera encontrar

Sem os bundles o mod carrega e registra as receitas, mas **cada item aparece sem
modelo e sem ícone**. Cada módulo procura uma subpasta com o nome dele, ao lado
do DLL:

```text
BepInEx/plugins/AlterraHub/
    FCS_AlterraHub/              ← + Audio/ (voz da Ava, missões)
    FCS_CyclopsUpgradeConsole/
    FCS_EnergySolutions/
    FCS_HomeSolutions/           ← + o JukeBox (mp3 do jogador)
    FCS_LifeSupportSolutions/
    FCS_ProductionSolutions/
    FCS_StorageSolutions/
```

São **7 pastas** e **6 bundles nomeados** — `fcsalterrahubbundle`,
`fcsenergysolutionsbundle`, `fcshomesolutionsbundle`,
`fcslifesupportsolutionsbundle`, `fcsproductionsolutionsbundle`,
`fcsstoragesolutionsbundle`. O `FCS_CyclopsUpgradeConsole` **não carrega bundle
próprio**: recebe um por propriedade (`CUCModelPrefab.Bundle`).

## 2. As 19 DLLs — auditadas uma a uma

Nenhuma é criação do FCStudios e nenhuma é asset de jogo. Todas são framework de
terceiros, versionadas pelo autor por conveniência de build.

| nome | origem | tipo | autor | licença | redistribuímos? |
| --- | --- | --- | --- | --- | --- |
| `SMLHelper.dll` | Libs/{SN,BZ}_{Stable,Exp} | framework legado | SMLHelper contributors | GPL-3.0 | ❌ não — substituído pela ponte |
| `QModInstaller.dll` | idem | carregador legado | QModManager | GPL-3.0 | ❌ não — substituído pelo BepInEx |
| `BepInEx.dll` | idem | carregador | BepInEx | LGPL-2.1 | ❌ não — quem joga já tem |
| `0Harmony.dll` | idem | patching | pardeike / BepInEx | MIT | ❌ não — vem com o BepInEx |
| `MoreCyclopsUpgrades.dll` | Libs/SN_{Stable,Exp} | **outro mod** | PrimeSonic | ver nota | ⚠️ **só referência de compilação** |
| `NAudio.dll` | Libs/SN_Stable | áudio | Mark Heath | MIT | ⚠️ **só referência de compilação** |

**Nota sobre as duas últimas.** `FCS_CyclopsUpgradeConsole` depende do
`MoreCyclopsUpgrades` e o JukeBox do `FCS_HomeSolutions` depende do `NAudio`. Nós
**não as versionamos e não as empacotamos**: o CI baixa as duas do próprio
repositório do FCS, no commit fixado, apenas para compilar. `refs/`, `artifacts/`
e `dist/` são ignorados pelo git — verificado com `git check-ignore`.

O empacotador recusa o build se qualquer DLL que não seja `Unhinged.*` aparecer
dentro de um pacote. Não é promessa: é uma verificação que roda.

## 3. Áudio referenciado no código (mas não incluído)

O `FCS_AlterraHub` carrega narração por caminho de arquivo — `AH-Mission01-Pt1.wav`,
`PDA_Instructions.mp3`, `ElectricalBoxesNeedFixing.mp3` e outros, todos sob
`FCS_AlterraHub/Audio/`. **Esses arquivos não estão no upstream** e não são nossos
para distribuir. Sem eles o mod não quebra: o carregamento de som falha e segue.

## 4. Regra aplicada

> Se o asset puder ser redistribuído: portar.
> Se não puder: não incluir.
> **Se houver dúvida: não assumir permissão.**

Não houve caso de dúvida, porque não houve caso: o upstream não traz asset
nenhum. A licença MIT do FCStudios cobre o **código**, que é o que este porte
carrega, com a atribuição preservada em `CREDITOS.md` e `LICENSE-FCS.txt`.

## 5. Como quem joga obtém os assets

Manualmente, do FCS original que já tenha instalado: copiar as 7 pastas de
`QMods/` para `BepInEx\plugins\AlterraHub\`, mantendo o nome intacto. Está no
`LEIA-ME.md` do pacote.

Nós não hospedamos, não espelhamos e não empacotamos esses arquivos.

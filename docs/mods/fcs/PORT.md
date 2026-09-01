# FC Studios modernizado — o que foi feito

**Pacote:** `Subnautica Unhinged — Alterra Hub (FCStudios)` · `com.subnauticaunhinged.alterrahub`
**Versão:** 1.1.0
**Origem:** https://github.com/ccgould/FCStudios_SubnauticaMods
**Commit/tag upstream:** `4275d847de6e0f24c711b4b2a9f4308c10ea8248` — *"AlterraHub Mod Suite V1.0.2"*, 19/08/2022
**Licença upstream:** MIT · `Copyright (c) 2020 Field Creator Studios`
**Subnautica suportado:** build **82304** — ver [`SUBNAUTICA-TARGET-BUILD.md`](../../SUBNAUTICA-TARGET-BUILD.md)

## ⚠️ Nível de verificação — leia antes de qualquer outra coisa

> ## `Build verified`
>
> **NÃO** `In-game tested`. **NÃO** `Automated tests verified`.

O ambiente onde este porte foi feito é **Linux, sem Subnautica instalado**. Nada
foi aberto no jogo. O que está provado, e como:

| verificação | resultado | como |
| --- | --- | --- |
| compila contra as assemblies reais da build 82304 | ✅ **0 erros** | 638 arquivos, `dotnet build -c Release` |
| todo `[HarmonyPatch]` encontra o alvo | ✅ **66/66** | `tools/VerificarPatches`, por metadata |
| alvos imperativos (`AccessTools`) existem | ✅ **2/2** | idem, modo `MEMBROS=` |
| reflexão por string em tipo do jogo | ✅ **4/4** | idem |
| todo interruptor de módulo casa com um `[QModCore]` | ✅ **7/7** | `build/conferir-modulos.sh` |
| ZIP com `BepInEx/` na raiz | ✅ | conferido no asset publicado, baixado de volta |
| **comportamento em jogo** | ❌ **não verificado** | — |
| **compatibilidade de save** | ❌ **não verificado** | — |
| **assinatura** dos patches | ❌ **não verificado** | o alvo existe; se um parâmetro mudou de tipo, só o runtime diz |
| **carregamento dos asset bundles** | ❌ **não verificado** | os bundles não são nossos — ver [`FCS-ASSET-MIGRATION.md`](ASSETS.md) |

## Módulos portados

Os sete viram **um assembly** (`Unhinged.AlterraHub.dll`, ~1,6 MB), carregado por
um `[BepInPlugin]` só.

| módulo | .cs | linhas | interruptor |
| --- | ---: | ---: | --- |
| Alterra Hub (base) | 232 | 31 742 | `EnableAlterraHub` |
| Home Solutions | 153 | 26 112 | `EnableHome` |
| Production Solutions | 102 | 16 674 | `EnableProduction` |
| Energy Solutions | 66 | 11 044 | `EnableEnergy` |
| Storage Solutions | 57 | 8 014 | `EnableStorage` |
| Life Support Solutions | 33 | 3 694 | `EnableLifeSupport` |
| Cyclops Upgrade Console | 12 | 831 | `EnableCyclops` |

`FCSDemo` (10 arquivos) ficou de fora — é projeto de demonstração.

## Incompatibilidades resolvidas

| # | o que era | o que foi feito |
| --- | --- | --- |
| 1 | QModManager carregava os `[QModCore]` | `LegacyModLoader` ocupa esse lugar, respeitando pré-patch → patch → pós-patch, com ordem determinística |
| 2 | SMLHelper 2.15 (137 arquivos) | ponte `Unhinged.Legacy` reimplementa `SMLHelper.V2.*` sobre o Nautilus |
| 3 | `harmony.PatchAll(assembly)` em 7 módulos | **cada patch era aplicado 7×** (129 classes × 7). Trocado por `PatchModule` por namespace |
| 4 | `unlockAtStart` | padrão **invertido** entre os frameworks: `false` no Nautilus, `true` no SMLHelper. A ponte usava a sobrecarga curta → **todo item nascia bloqueado** |
| 5 | caminhos de asset achatados no merge | cada módulo volta a procurar a subpasta com o nome dele |
| 6 | ZIP com pasta de topo | a raiz do arquivo era `AlterraHub-vX/`, não `BepInEx/` — **o mod nunca era carregado** |
| 7 | `BioReactorHandler` | **não existe** no Nautilus; implementado na ponte, escrevendo em `BaseBioReactor.charge` |
| 8 | `Oculus.Newtonsoft.Json` | resolvido pelo próprio `#if` do autor — compilamos o `#else` |

### Sobre o nº 6, e o que ele ensinou

Os defeitos 1, 3, 4 e 5 são reais e continuam corrigidos, mas **nenhum deles
tinha sido exercitado**: até a 1.0.7 o assembly nunca chegou a ser carregado. Três
diagnósticos seguidos saíram de leitura de código e erraram a causa. Quem
respondeu foi o log do jogo — pela **ausência** de qualquer linha nossa, inclusive
do `Loading [...]` que o BepInEx escreve antes de qualquer código nosso rodar.

Está registrado em [`PORTE-LEGADO.md`](../../PORTE-LEGADO.md) §3.8.

## Funcionalidades não portadas

Marcadas, não apagadas. Detalhe em [`FCS-UPSTREAM-AUDIT.md`](AUDIT.md) §7.

| marcador | o quê |
| --- | --- |
| `LEGACY_API` | painel de opções e comandos de console: os atributos compilam, mas **ainda não são registrados em jogo** — as opções são ignoradas sem erro visível |
| `REQUIRES_RUNTIME_TEST` | `ModUtils.Save` / `LoadSaveData` — mexe em dado de save; errar corrompe o save de quem joga |
| `MISSING_DEPENDENCY` | patch em `SubnauticaMap.PingMapIcon`, que é **outro mod**; já protegido por `if (type != null)` |
| `GAME_API_CHANGED` | `BuilderTool.constructText`/`deconstructText` não existem na 82304 — inerte, todas as ocorrências comentadas |

## Configuração

Um mecanismo (`Config.Bind` do BepInEx), um arquivo `.cfg`:

```ini
[1. Compatibilidade]
ForcarComPilhaLegada = false   # carregar junto com QModManager/SMLHelper

[2. Modulos]
EnableFCS         = true       # chave mestra
EnableAlterraHub  = true       # BASE — os outros seis dependem dele
EnableEnergy      = true
EnableHome        = true
EnableLifeSupport = true
EnableProduction  = true
EnableStorage     = true
EnableCyclops     = true       # exige o mod MoreCyclopsUpgrades
```

Desligar `EnableAlterraHub` com dependentes ligados **não carrega nada** e diz por
quê: eles registram nos serviços da base, e sem ela o resultado não é erro limpo.

> ⚠️ O `namespace` do Cyclops é `CyclopsUpgradeConsole` — **sem** o `FCS_` que a
> pasta tem. Eu escrevi o nome da pasta e o `EnableCyclops` virou um interruptor
> morto: aparecia no `.cfg`, o jogador desligava, e o módulo carregava assim
> mesmo. `build/conferir-modulos.sh` agora reprova o build nesse caso.

## Dependências

| depende de | como |
| --- | --- |
| BepInEx 5.4.21+ | obrigatório |
| Nautilus 1.0.0-pre.53 | `HardDependency` |
| MoreCyclopsUpgrades | só para `EnableCyclops`; referência de compilação, **não redistribuída** |
| NAudio | JukeBox do Home Solutions; idem |
| asset bundles do FCS | **não incluídos** — cópia manual |

⛔ **Não conviva com QModManager/SMLHelper.** O plugin se recusa a carregar e diz
por quê. Ligar `ForcarComPilhaLegada` é por conta de quem liga.

## Atualizar quando o Subnautica mudar

```text
UPSTREAM VERSION → COMPARE → API CHANGES → PORT → TEST → RELEASE
```

Na prática, aqui:

1. `git ls-remote` no upstream do FCS — hoje ele está **parado em 2022**, então
   normalmente não há nada a comparar.
2. Ver se saiu `Subnautica.GameLibs` nova no feed do BepInEx; trocar a versão no
   `.csproj` e a `NAUTILUS_TAG` no workflow.
3. `tools/VerificarPatches/rodar.sh` + `build/conferir-modulos.sh` — os dois
   reprovam o build se um alvo ou um interruptor deixou de existir. É o passo que
   transforma "o jogo mudou" em erro de build, em vez de bug na mão de quem joga.
4. Só então empacotar e publicar.

# Auditoria do upstream FCStudios

**Fonte:** https://github.com/ccgould/FCStudios_SubnauticaMods
**Commit auditado:** `4275d847de6e0f24c711b4b2a9f4308c10ea8248` — *"AlterraHub Mod Suite V1.0.2"*, **19/08/2022**
**Licença upstream:** MIT · `Copyright (c) 2020 Field Creator Studios`

> ⚠️ **O upstream está parado.** `git ls-remote` confirma que `4275d84` é o topo
> do `master` — não há branch nem commit posterior. Não existe versão nova para
> rebasear: a modernização é inteiramente deste repositório.

## 1. Inventário

**667** arquivos `.cs`, **99 173** linhas.

| projeto | .cs | linhas | vendorizado? |
| --- | ---: | ---: | --- |
| `FCS_AlterraHub` | 232 | 31 742 | ✅ |
| `FCS_HomeSolutions` | 153 | 26 112 | ✅ |
| `FCS_ProductionSolutions` | 102 | 16 674 | ✅ |
| `FCS_EnergySolutions` | 66 | 11 044 | ✅ |
| `FCS_StorageSolutions` | 57 | 8 014 | ✅ |
| `FCS_LifeSupportSolutions` | 33 | 3 694 | ✅ |
| `FCS_CyclopsUpgradeConsole` | 12 | 831 | ✅ |
| `FCSDemo` | 10 | 889 | ❌ projeto de demonstração, fora do escopo |

Conferência da conta: `667 − 10 (FCSDemo) + 1 (nosso Plugin.cs) = 658` arquivos no
repositório, dos quais o MSBuild compila **638** (a diferença são `AssemblyInfo.cs`
e 14 arquivos que **o próprio autor também não compilava** — ver §4).

## 2. Uso de API legada, medido

| API | arquivos que usam | situação |
| --- | ---: | --- |
| `UnityEngine` | 494 | permanece — Unity 2019.4.36 |
| `SMLHelper` | 137 | ❌ legado → ponte `Unhinged.Legacy` |
| `Harmony` | 54 | ⚠️ existe nas duas eras, API mudou → HarmonyX 2.7 |
| `Oculus.Newtonsoft` | 30 | ⚠️ **só dentro de `#if SUBNAUTICA_STABLE`** — ver §3 |
| `QModManager` | 11 | ❌ legado → BepInEx |

## 3. O `#if` que decide metade da resposta

O FCS é compilado condicionalmente, e **o autor já havia previsto a migração**:

```csharp
#if SUBNAUTICA_STABLE
using Oculus.Newtonsoft.Json;   // build antiga: Newtonsoft embutido nesse namespace
#else
using Newtonsoft.Json;          // namespace normal — o que o jogo atual tem
#endif
```

Definimos **apenas `SUBNAUTICA`**, então compila o `#else`. `Oculus.Newtonsoft.Json`
não existe na build 82304 (confirmado por metadata), e não precisa existir.

| símbolo | usos | definido? |
| --- | ---: | --- |
| `SUBNAUTICA` | 199 | ✅ |
| `SUBNAUTICA_STABLE` | 112 | ❌ |
| `DEBUG` | 18 | ❌ (Release) |
| `BELOWZERO` | 6 | ❌ |
| `SUBNAUTICA_EXP` | 3 | ❌ |

> ⚠️ **Grep não enxerga `#if`.** Buscar `Oculus.Newtonsoft` acha 28 linhas não
> comentadas e sugere um porte quebrado — são 28 linhas desabilitadas pelo
> pré-processador. Quem responde de verdade é o MSBuild (`-getItem:Compile`).

## 4. Código morto que o autor deixou no repositório

14 arquivos estão no repositório mas **não aparecem em nenhum `.csproj` do autor**
— os projetos dele usam `<Compile Include>` explícito, não glob. Nós usamos glob,
então precisamos removê-los à mão para reproduzir o mesmo conjunto:

`Mono/AlterraHub/` (7 arquivos: controller, display, encyclopedia, painéis, abas) ·
`Mods/Stairs/Patchers/` (6 patchers) · `Patches/InGameMenuPatcher.cs`

Cada `<Compile Remove>` foi conferido contra os `.csproj` do upstream: **nenhum
deles era compilado lá**. Nada de funcionalidade foi perdido.

Há ainda **33 sítios de reflexão comentados** no fonte — código morto que o autor
deixou como referência.

## 5. Dependências externas

O upstream versiona **19 DLLs**, todas de framework/terceiros — nenhuma é criação
do FCS e nenhuma é asset de jogo:

| DLL | pastas | o que é |
| --- | --- | --- |
| `SMLHelper.dll` | 4 | framework legado |
| `QModInstaller.dll` | 4 | carregador legado |
| `BepInEx.dll` | 4 | carregador |
| `0Harmony.dll` | 4 | patching |
| `MoreCyclopsUpgrades.dll` | 2 | **outro mod** — usado por `FCS_CyclopsUpgradeConsole` |
| `NAudio.dll` | 1 | MIT — usado pelo JukeBox do `FCS_HomeSolutions` |

As quatro pastas são `SN_Stable`, `SN_Exp`, `BZ_Stable`, `BZ_Exp` (Subnautica e
Below Zero × estável e experimental). Também há um `NStrip.exe` (ferramenta de
publicização de assemblies) — não usamos.

**Nós não versionamos nenhuma delas.** `MoreCyclopsUpgrades.dll` e `NAudio.dll`
são baixadas do próprio repositório do FCS no commit fixado, só como referência de
compilação; `refs/`, `artifacts/` e `dist/` são ignorados pelo git (verificado com
`git check-ignore`).

## 6. Matriz legado → moderno (Fase 2)

| Componente | Tecnologia antiga | API atual | Compat. | Ação tomada |
| --- | --- | --- | --- | --- |
| Loader | QModManager 4.x | BepInEx 5.4.21 | ❌ | `Plugin.cs` com `[BepInPlugin]`; `LegacyModLoader` executa os `[QModCore]` |
| Framework | SMLHelper 2.15 | Nautilus 1.0.0-pre.53 | ❌ | ponte `Unhinged.Legacy` reimplementa `SMLHelper.V2.*` sobre o Nautilus |
| Patching | Harmony (`PatchAll`) | HarmonyX 2.7 | ⚠️ | `PatchModule` por namespace — `PatchAll` aplicava cada patch **7×** |
| Registro de item | `Spawnable`/`Craftable` | `PrefabInfo.WithTechType` | ⚠️ | ⚠️ **padrão invertido**: `unlockAtStart` é `false` no Nautilus e era `true` no SMLHelper |
| Serialização | `Oculus.Newtonsoft.Json` | `Newtonsoft.Json` | ✅ | já resolvido pelo `#if` do autor |
| Recipes / PDA / TechType | handlers do SMLHelper | handlers do Nautilus | ⚠️ | reexportados pela ponte |
| `BioReactorHandler` | SMLHelper | **não existe** no Nautilus | ❌ | implementado na ponte, escrevendo em `BaseBioReactor.charge` |
| Opções / console | `OptionsPanelHandler` | `Nautilus.Options` | ⚠️ | ⚠️ atributos compilam mas **ainda não registram** — ver §7 |
| Assets | bundles fora do repo | idem | — | não redistribuídos — ver `FCS-ASSET-MIGRATION.md` |

## 7. O que ficou marcado, não apagado

| marcador | onde | motivo |
| --- | --- | --- |
| `LEGACY_API` | `OptionsPanelHandler`, `ConsoleCommandsHandler` | os atributos existem e compilam, mas **o painel e os comandos ainda não são registrados em jogo**. Um mod portado agora tem as opções ignoradas, sem erro visível. |
| `REQUIRES_RUNTIME_TEST` | `ModUtils.Save` / `LoadSaveData` | mexe em **dado de save**. Errar corrompe o save de quem joga; merece verificação própria, não dedução. |
| `MISSING_DEPENDENCY` | `SubnauticaMap.PingMapIcon` | alvo de um patch imperativo que aponta para **outro mod**, não para o jogo. Já protegido por `if (type != null)`. |
| `GAME_API_CHANGED` | `BuilderTool.constructText` / `deconstructText` | **não existem** na build 82304. Inerte: todas as ocorrências estão comentadas, em arquivo que o autor também não compilava. |
| `LEGACY_API` (bug do autor) | `typeof(bool).GetField("shouldPlayIntro")` | bug real do upstream — deveria ser `typeof(PDA)`. Inerte: campo declarado e nunca lido. **Não corrigido de propósito**: adivinhar o tipo certo é o que este porte não faz. |

## 8. Estado verificado do porte

| verificação | resultado |
| --- | --- |
| compila contra a build 82304 | **0 erros** (638 arquivos) |
| alvos de `[HarmonyPatch]` | **66**, todos existem |
| alvos imperativos (`AccessTools`) | **2**, ambos existem |
| reflexão por string em tipo do jogo | **4** vivos, todos existem |
| layout do ZIP (`BepInEx/` na raiz) | conferido no asset publicado |
| **testado em jogo** | ❌ **não** — ambiente Linux, sem Subnautica |

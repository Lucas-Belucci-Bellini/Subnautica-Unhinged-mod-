# Log de compatibilidade — APIs legadas encontradas

Cada linha é uma API que mudou entre a era do FCS (2022, série 71xxx) e a build
82304. Serve para a próxima vez que o Subnautica atualizar: é onde olhar primeiro.

Números medidos por comparação arquivo a arquivo com o upstream em `4275d84`.

| Legacy API | Modern API | Arquivo (exemplo) | Linha | Problema | Correção |
| --- | --- | --- | ---: | --- | --- |
| `SMLHelper.V2.Crafting.TechData` | mesmo tipo, com **alias** | `FCS_AlterraHub/Configuration/Mod.cs` | 35 | o jogo passou a ter um `TechData` **global**; o nome colide e o compilador não resolve | `using TechData = SMLHelper.V2.Crafting.TechData;` no topo — **76 arquivos** |
| `CraftData.GetItemSize` · `GetEquipmentType` · `GetCraftTime` | `TechData.*` | `FCS_AlterraHub/Helpers/TechDataHelpers.cs` | 190 | os membros migraram de classe | chamada trocada — **14 sítios** |
| `ITechData` indexador | `GetIngredient(i)` | `FCS_AlterraHub/Helpers/TechDataHelpers.cs` | 143 | o indexador saiu da interface | `it.GetIngredient(i)` — **4 sítios** |
| `Ocean.main.GetDepthOf(go)` | `Ocean.GetDepthOf(go)` | `FCS_AlterraHub/Helpers/WorldHelpers.cs` | 426 | virou **estático**; `Ocean.main` não existe mais | prefixo removido — **3 sítios** |
| `HandReticle.SetInteractText(...)` | sobrecargas novas / `SetInteractTextRaw` | `FCS_AlterraHub/Mono/SearchField.cs` | 33 | assinatura mudou (inclusive a forma de 5 `bool`) | ponte `HandReticleCompat` no namespace global + chamadas ajustadas — **8 sítios** |
| `GameInput` — nome de tecla | `GetBinding` + `GetDisplayText` | `FCS_HomeSolutions/Mods/Curtains/Mono/CurtainController.cs` | 224 | a API de binding foi reescrita | `UnhingedInput.GetBindingName(Button, BindingSet)` — **11 sítios** |
| `Toggle.Set(bool, bool)` | `Toggle.isOn` | `FCS_ProductionSolutions/Mods/AutoCrafter/Patches/PowerIndicatorPatch.cs` | 259 | o método ficou **inacessível** | leitura/escrita por `isOn` — **3 sítios** |
| `List.AddIfNotPresent` · `Queue.TryDequeue` | — | `FCS_HomeSolutions/Mods/Stove/Mono/StoveController.cs` | 124 | métodos de instância **removidos** do jogo | extensões no namespace global (`CollectionsCompat`) — **2 sítios** |
| lista de comida cozida | derivada do enum | `FCS_AlterraHub/Mono/BaseManager.cs` | 1201 | a constante sumiu | `UnhingedFood.CookedCreatureList`, **derivada do `TechType` real** (14 pares, conferidos) — **5 sítios** |
| `harmony.PatchAll(assembly)` | `PatchModule(assembly, ns)` | `FCS_AlterraHub/QPatch.cs` | 76 | com 7 módulos no mesmo assembly, **cada patch era aplicado 7×** | patch por namespace — **10 sítios** |
| `Assembly.Location` → pasta do mod | `ModuleFolder(asm, nome)` | `FCS_AlterraHub/API/FCSAssetBundlesService.cs` | 32 | ao fundir 7 mods num DLL, todos passaram a apontar para a **mesma** pasta | subpasta por módulo — **10 sítios** |
| `Oculus.Newtonsoft.Json` | `Newtonsoft.Json` | — | — | namespace da build antiga | **nada a fazer**: já estava sob `#if SUBNAUTICA_STABLE`, e compilamos o `#else` |
| `QModManager` (carregador) | BepInEx `[BepInPlugin]` | `src/mods/AlterraHub/Plugin.cs` | 17 | `[QModCore]` só marca código; quem invocava não existe mais | `LegacyModLoader` invoca as três fases na ordem original |
| `SMLHelper.V2.*` (137 arquivos) | Nautilus | `src/Unhinged.Legacy/` | — | framework inteiro substituído | ponte reimplementa os namespaces sobre o Nautilus |
| `BioReactorHandler` | **não existe** | `src/Unhinged.Legacy/Handlers/CraftAndTechHandlers.cs` | — | o Nautilus não tem equivalente | implementado na ponte, escrevendo em `BaseBioReactor.charge` |
| `PrefabInfo.WithTechType(3 args)` | sobrecarga com `unlockAtStart` | `src/Unhinged.Legacy/Assets/ModPrefab.cs` | — | **padrão invertido**: `false` no Nautilus, `true` no SMLHelper → todo item nascia bloqueado | valor passado explicitamente |

## Onde isso é conferido automaticamente

`tools/VerificarPatches` resolve **por metadata**, contra as assemblies reais:

- 66 alvos de `[HarmonyPatch]`
- 2 alvos imperativos (`AccessTools.Method` / `AccessTools.Field`)
- 4 buscas por string em tipo do jogo (`typeof(Builder).GetField("…")`)

Se o jogo renomear qualquer um deles, o **build falha** — em vez de o jogador
descobrir. Roda em todo PR (`verificar.yml`) e antes de publicar.

⚠️ O que essa conferência **não** cobre: a **assinatura**. O alvo existe; se um
parâmetro mudou de tipo, só o runtime reclama. É a razão de a validação de runtime
ser obrigatória, e de ela ainda não ter acontecido.

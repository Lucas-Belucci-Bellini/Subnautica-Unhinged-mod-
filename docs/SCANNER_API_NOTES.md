# Nota de API — Sala de Scanner

> Exigência do briefing: *"Antes de implementar, produzir uma nota curta identificando
> as classes, eventos, patches Harmony e APIs Nautilus realmente existentes. Se a API
> não for confirmada, parar e documentar a dúvida em vez de inventar chamadas."*
>
> **Tudo marcado ✅ abaixo foi lido do metadata real das assemblies.** Nada aqui foi
> escrito de memória. O que não pôde ser confirmado está na seção "Dúvidas em aberto",
> não no corpo do plano.

## Como isto foi verificado

Sem acesso à máquina Windows, as assemblies do jogo vieram do pacote público
**`Subnautica.GameLibs` 82304.0.0-r.0`** (autor: MrPurple6411, feed
`https://nuget.bepinex.dev/v3/index.json`) — as mesmas assemblies de referência que o
Nautilus usa. Os nomes e assinaturas abaixo foram extraídos com um leitor de metadata
(`System.Reflection.Metadata`) sobre `Assembly-CSharp.dll`.

⚠️ **Descompasso de versão:** o jogo instalado é build **83031**; o GameLibs público mais
recente é **82304** (só existem 4 versões publicadas: 71137, 71137.0.0.1, 71288, 82304).
Para compilar isso não é problema, mas **a confirmação final de qualquer assinatura deve
ser feita contra o `Assembly-CSharp.dll` local do build 83031** antes de escrever patches.

## Camada moderna — confirmada

| Item | Valor | Status |
| --- | --- | --- |
| Target framework | `net472` | ✅ (Nautilus e o Scan for Anything usam) |
| Unity | `2019.4.36` | ✅ (pacote `UnityEngine.Modules` fixado pelo Nautilus) |
| GUID do Nautilus | `com.snmodding.nautilus` | ✅ |
| BepInEx | `BepInEx.Core` 5.4.21 (BepInEx **5**, não 6) | ✅ |
| Nautilus | **1.0.0-pre.53** (compilado da fonte) | ✅ é a versão instalada **e** a do master do Nautilus |

## `MapRoomFunctionality` — a sala de scanner ✅

**Constantes (valores reais lidos do metadata):**

| Constante | Valor |
| --- | --- |
| `defaultRange` | **300** m |
| `rangePerUpgrade` | **50** m |
| `mapScanRadius` | **500** m |
| `baseScanTime` | **14** s |
| `scanTimeReductionPerUpgrade` | **3** s |
| `powerPerSecond` | 0.5 |
| `idlePowerPerSecond` | 0.15 |

➡️ **O alcance máximo vanilla é 500 m** (300 + 4×50, coerente com `mapScanRadius = 500`).
Os 5 km pedidos são **10× o alcance** e, como área cresce com o quadrado do raio,
**100× a área varrida**. Este número é a razão de existir o escalonamento e o teto de
resultados na configuração.

> ⛔ **`defaultRange` e `rangePerUpgrade` são `const` (`Literal`).** O compilador as
> embute nos call sites: **o Harmony não consegue patchá-las**. Qualquer mudança de
> alcance tem que passar por `GetScanRange()` ou por `UpdateScanRangeAndInterval()`
> escrevendo no campo `scanRange`. Tentar "mudar a constante" não funciona.

**Métodos úteis:** `GetScanRange()`, `UpdateScanRangeAndInterval()`, `StartScanning(TechType)`,
`ObtainResourceNodes(TechType)`, `UpdateScanning()`, `GetNodes()` → `IList<ResourceInfo>`,
`GetDiscoveredNodes(ICollection<ResourceInfo>)`, `UpdateBlips()`, `UpdateCameraBlips()`,
`GetMapRoomsInRange(Vector3, float, ICollection<MapRoomFunctionality>)`,
`OnResourceDiscovered(ResourceInfo)`, `OnResourceRemoved(ResourceInfo)`, `GetActiveTechType()`.

**Campos úteis:** `scanRange`, `scanInterval`, `typeToScan`, `numNodesScanned`, `scanActive`,
`timeLastScan`, `hologramRadius`, `onScanRangeChanged`, `storageContainer`, `upgradeSlots`,
`powerConsumer`, `resourceNodes`, `mapBlips`, `cameraBlips`, e o estático `mapRooms`.


## `MapRoomCamera` / `MapRoomScreen` — o drone ✅

Verificado no mesmo metadata, ao implementar o mod.

| Constante | Valor | Onde |
| --- | --- | --- |
| `maxCameraDistance` | **500** m | `MapRoomScreen` |
| `acceleration` | 20 | `MapRoomCamera` |
| `sidewaysTorque` | 45 | `MapRoomCamera` |
| `stabilizeForce` | 15 | `MapRoomCamera` |

> ⛔ **`maxCameraDistance` também é `const`** — mesma armadilha do alcance do scanner.
> Não há campo em runtime para escrever: o 500 está embutido no IL de
> `MapRoomCamera.CanBeControlled(MapRoomScreen)`.

**Membros usados:** `CanBeControlled(MapRoomScreen)` → `bool`,
`GetScreenDistance(MapRoomScreen)` → `float`, `ControlCamera(MapRoomScreen)`,
`FreeCamera(bool)`, `Update()`, `IsControlled()`, `screenEffectModel`.

**`MapRoomCameraScreenFX`:** propriedade `noiseFactor` (`float`), campo `_noiseFactor`.

### Correção que o compilador fez, e vale registrar

`MapRoomCamera.screenEffectModel` é um **`GameObject`**, não o componente
`MapRoomCameraScreenFX`. Eu havia assumido que era o componente; escrever a suposição em
código e compilar contra a assembly real desmentiu na hora — antes de virar
`NullReferenceException` em jogo. O componente sai de
`screenEffectModel.GetComponentInChildren<MapRoomCameraScreenFX>(true)`, resolvido uma
vez ao assumir o controle (buscar por quadro num `Update` seria desperdício).

**Compilar contra o metadata real é uma forma barata de testar suposição.**

## Implementado em `src/mods/ScannerRoom` (v0.1.0)

| Requisito do operador | Como |
| --- | --- |
| Drone ≥ 1000 m | Transpiler em `CanBeControlled`, trocando o literal 500. |
| Scanner 5 km com todos os chips | Postfix em `UpdateScanRangeAndInterval` + `GetScanRange`. |
| Degradação visual do drone | Postfix em `Update`, escrevendo `noiseFactor`. Só a imagem — blips e rastreamento intactos. |

**Contagem de chips derivada, não recontada.** A vanilla já calcula `300 + n×50`, então
`n = (alcance − 300) / 50`. Isso evita duplicar a regra de quais itens contam como
upgrade — e se o jogo mudar essa regra, o mod acompanha sozinho.

**O transpiler se auto-reporta.** Um transpiler que não casa com nada falha em
silêncio; o patch conta as substituições e o plugin escreve no log se foram zero. Sem
isso, o sintoma seria "o drone continua parando em 500 m" sem nada no log — o tipo de
defeito que custa horas.

## `ResourceTrackerDatabase` — o registro que o scanner consulta ✅

É **aqui** que mora a detecção; a sala de scanner só lê deste registro estático.

- `GetNodes(Vector3, float, TechType, ICollection<ResourceInfo>)` ← **consulta por raio**;
  é a chamada natural para os 5 km.
- `GetNodes(TechType)` → `ICollection<ResourceInfo>`
- `GetTechTypesInRange(Vector3, float, ICollection<TechType>)` ← alimenta os **filtros por categoria**
- `HasTechTypeNearby(Vector3, float, TechType)`, `HasNodeNearby(...)`
- `Register(string, Vector3, TechType)` / `Unregister(string, TechType)`
- `IsDetectableTechType(TechType)`, `GetDetectableTechTypes()`
- Campos estáticos: `resources`, **`detectableTechTypes`**, **`undetectableTechTypes`**, `scannedTechsTooltips`
- Eventos: `onResourceDiscovered`, `onResourceRemoved` (privados, com `add_`/`remove_` públicos)

**`ResourceTrackerDatabase.ResourceInfo`** (tipo aninhado) tem exatamente três campos:
`uniqueId`, `techType`, `position`.

➡️ **Não existe campo de velocidade nem de direção.** O requisito "mostrar movimento"
não sai de graça: tem que ser derivado de amostras sucessivas de `position` (ver abaixo).

## `ResourceTracker` — como um objeto entra no registro ✅

`Register()`, `Unregister()`, **`StartUpdatePosition()`**, `StopUpdatePosition()`,
`UpdatePosition()`; campos `techType`, `overrideTechType`, `prefabIdentifier`, `pickupable`,
`rb`, `uniqueId`.

➡️ `StartUpdatePosition()` é o mecanismo vanilla para alvos **que se movem**. Leviatã parado
no registro seria um blip mentiroso — mas cada alvo em movimento também é custo por frame.
Este é o principal fator de desempenho do protótipo, junto com o raio.

## Drones — degradação visual ✅

**`MapRoomCameraScreenFX`** tem a propriedade **`noiseFactor` (float)**, com
`get_noiseFactor`/`set_noiseFactor`, e `OnRenderImage(RenderTexture, RenderTexture)`.

➡️ Este é o encaixe exato do requisito "degradação visual progressiva depois de 2 km":
dá para escrever em `noiseFactor` em função da distância **sem tocar no rastreamento
lógico**, que vive no `ResourceTrackerDatabase`. Imagem e dado ficam naturalmente separados,
como o briefing pede.

**`MapRoomCamera`** (o drone): `GetCamerasInRange(Vector3, float, ICollection<MapRoomCamera>)`,
**`GetScreenDistance(MapRoomScreen)`**, `GetDepth()`, `IsControlled()`, `ControlCamera(MapRoomScreen)`,
`FreeCamera(bool)`, `UpdatePingLabel()`, `Update()`, `FixedUpdate()`; campos `screenEffectModel`,
`energyMixin`, `liveMixin`, `active`, `screen`; estático `cameras`.

## Leviatãs — verificação nome a nome

| Pedido no briefing | Classe | `TechType` | Status |
| --- | --- | --- | --- |
| Reaper | `ReaperLeviathan` | `ReaperLeviathan = 2540` | ✅ |
| Ghost | `GhostLeviathan` | `GhostLeviathan = 2562`, `GhostLeviathanJuvenile = 2565` | ✅ |
| Sea Dragon | `SeaDragon` | `SeaDragon = 2553` | ✅ |
| Reefback | `Reefback` / `ReefbackCreature` | `Reefback = 2518` | ✅ |
| **Shadow** | — | — | ❌ **não existe neste jogo** |

⚠️ **Shadow Leviathan é criatura de _Subnautica: Below Zero_, não do Subnautica base.**
As únicas correspondências para "Shadow" em `Assembly-CSharp` são tipos de iluminação
(`LightShadowQuality`, `ShadowMapCopy`, `ShadowQualityPair`). **Não dá para rastreá-lo
aqui** — e inventar um `TechType` para ele seria exatamente o tipo de chamada fantasia que
o briefing proíbe. Se a intenção era Below Zero, isso muda o alvo do projeto e precisa de
decisão explícita.

Extra confirmado e provavelmente desejável: **`SeaTreader = 2536`** (Sea Treader).
Existe também uma classe-marcador `Leviathan`, mas ela é **vazia** (só construtor) — serve
para *identificar* (`GetComponent<Leviathan>()`), não para ler dados.

## Arte prévia — `Scan for Anything` (já instalado na máquina) ✅

Fonte: <https://github.com/GreaterDane42/Subnautica-Mods> (autor: GreaterDane42), pasta
`Scan for More`. Mod moderno, BepInEx/Nautilus, `net472`. **Já está em `BepInEx\plugins`
do operador** — ou seja, é dependência de compatibilidade, não só referência.

O que ele faz, e que **não precisamos reimplementar**:

- `[HarmonyPatch(typeof(ResourceTracker), nameof(ResourceTracker.Start))] [HarmonyPostfix]`
  → registra fragmentos.
- `[HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.Awake))] [HarmonyPostfix]`
  → adiciona `ResourceTracker` via `EnsureComponent<ResourceTracker>()` a **qualquer coisa
  com vida**, usando `CraftData.GetTechType(gameObject)`.
- `[HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.Kill))] [HarmonyPostfix]`
  → `Unregister()`.

➡️ **Leviatãs têm `LiveMixin`.** Então o caminho "tornar leviatã detectável" **já existe e
está resolvido** por esse mod com `trackAllLife = true`. O Unhinged deve **integrar**, não
duplicar: dois mods adicionando `ResourceTracker` ao mesmo objeto é registro duplicado.

> Limitação declarada pelo próprio autor: só entram no scanner **fragmentos e objetos com
> vida**; fragmentos aparecem agrupados numa única entrada.

## Dúvidas em aberto — **não implementar antes de resolver**

1. **O registro só contém o que já foi carregado.** O README do Scan for Anything diz que a
   sala de scanner "só detecta objetos em áreas que você visitou desde que carregou o jogo",
   e recomenda o mod *More Resources Discovery* para contornar. Se isso valer,
   **um scanner de 5 km vai devolver muito menos que 5 km de conteúdo** — o limite real
   passa a ser o streaming de terreno, não o raio. **Medir antes de prometer 5 km.**
2. **Assinaturas contra o build 83031.** Confirmar `GetScanRange`, `UpdateScanRangeAndInterval`
   e `MapRoomCameraScreenFX.noiseFactor` no `Assembly-CSharp.dll` local antes de patchar.
3. **UI e filtros.** Existem `uGUI_MapRoomScanner`, `uGUI_MapRoomResourceNode` e `MapRoomScreen`,
   mas **ainda não abri os membros deles**. Os filtros por categoria e a exibição de
   nome/distância/direção dependem dessa leitura — hoje é dúvida, não plano.
4. **Custo de `StartUpdatePosition()` em escala.** Quantos alvos móveis o registro aguenta
   antes de custar frame? Sem medição, o teto de 200 resultados é um chute prudente.
5. **Conflito com os mods já instalados.** `Scan for Anything` (**1.0.4 — exatamente a
   versão cujo código foi lido aqui**), `Scanner Speed Multipler` 1.4.0, `Sonar Module` 2.1
   e `Resource Monitor` tocam nessa mesma área. O Scan for Anything **já resolve o
   rastreamento de leviatãs**: o Unhinged deve integrar, não duplicar. Falta confirmar o
   que o Scanner Speed Multiplier patcha — e se ele está sequer implantado.
   Detalhamento em [`MOD_COMPATIBILITY.md`](MOD_COMPATIBILITY.md).
6. **Licenças.** O **Nautilus é GPL-3.0-only**. Um overhaul distribuído que linka Nautilus
   precisa de uma decisão consciente de licenciamento para o Unhinged — isso não é detalhe
   de rodapé, é condição de distribuição.

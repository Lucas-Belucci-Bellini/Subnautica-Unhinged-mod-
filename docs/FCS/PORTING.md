# FCS — o que mudou do legado para o moderno

```
QModManager 4.4.4 + SMLHelper 2.15        BepInEx 5.4.21 + Nautilus 1.0.0-pre.53
        (2022, morto)              →              (build 82304, atual)
```

A estratégia **não** é tradução mecânica. É uma **ponte** — `Unhinged.Legacy` —
que reimplementa os namespaces `SMLHelper.V2.*` sobre o Nautilus. O código do FCS
segue lendo como o original; quem muda é o chão embaixo dele.

Por que ponte e não reescrita: são **667 arquivos** de código de terceiro que
funcionava. Reescrever à mão significaria reintroduzir bugs que o autor já tinha
consertado, sem meio de comparar. A ponte deixa o diff do porte pequeno o
bastante para ser auditável.

## O que a ponte cobre

| Legado | Moderno | Onde |
| --- | --- | --- |
| `[QModCore]` / `[QModPatch]` | invocados pelo `LegacyModLoader` | `LegacyModLoader.cs` |
| `SMLHelper.V2.Assets.ModPrefab` | `Nautilus.Assets.CustomPrefab` | `Assets/ModPrefab.cs` |
| `Spawnable.UnlockedAtStart` | `PrefabInfo.WithTechType(unlockAtStart:)` | idem |
| `ImageUtils.LoadSpriteFromFile` | Nautilus + **guarda de null** | `Utility/ImageUtils.cs` |
| `OptionsPanelHandler` | Nautilus + **tolerante a duplicata** | `Options/OptionsPanelHandler.cs` |
| `ConsoleCommandsHandler` | Nautilus + falha não-fatal | `Handlers/SpawnAndCommandHandlers.cs` |
| caminho de asset | `UnhingedModPaths` (5 layouts) | `GameCompat/ModPathsCompat.cs` |

## A regra que o porte aprendeu, três vezes

**Nada cosmético ou opcional pode derrubar o item nem o módulo.** Ícone, painel
de opções, comando de console e som degradam; não propagam.

As três violações, todas do mesmo mecanismo — código legado que usa
`Assembly.GetExecutingAssembly()` como **identidade** e não como conteúdo:

| Versão | Sintoma | Causa |
| --- | --- | --- |
| 1.0.5 | jogo não carregava | `PatchAll` aplicava 129 patches **7×** |
| 1.0.3 / 1.0.7 | itens sem modelo | as 7 pastas `Assets` colapsadas numa |
| 1.3.0 | 6 dos 7 módulos mortos | painel de opções colidindo na chave do assembly |

## Comportamento que mudou, e não pôde ser traduzido

| Onde | Decisão |
| --- | --- |
| **Créditos finais** | `EndCreditsManager` foi reescrito pelo jogo. `CreditsScrollSeconds = 200s` é **valor a calibrar** — o campo original sumiu e não é derivável. |
| **Tooltip de item** | O `RequestPermission` filtrava só o texto; no jogo atual texto e ícone vão no mesmo objeto, então a permissão passou a valer para os dois. |
| **Som ao coletar** | As três APIs de som de coleta foram apagadas do jogo. No lugar, o feedback padrão — que é a linha que o próprio autor deixara comentada ali. |

## Portões que rodam em CI

| Portão | O que prova |
| --- | --- |
| `VerificarPatches` | os **66** alvos de `[HarmonyPatch]` resolvem nas assemblies reais da 82304 |
| modo `MEMBROS=` | os 2 alvos imperativos e as 4 buscas por string existem |
| `conferir-modulos.sh` | nenhum interruptor de módulo é no-op |
| `empacotar.sh` | raiz do ZIP é `BepInEx/`, sem DLL de terceiro dentro |

Um check verde significa **"compila e todo alvo resolve"**. Não significa que
faz a coisa certa, e não substitui o jogo.

## ⚠️ `BLOCKED_EXTERNAL_GAME_SOURCE`

Este ambiente é **Linux, sem Subnautica**. Compila contra as assemblies de
referência (`Subnautica.GameLibs` 82304), mas não executa nada. Todo veredito de
comportamento, save/load e integração depende do runtime real — e a autoridade
final é o log do jogo, não a leitura de código. Foi assim que o P0 caiu.

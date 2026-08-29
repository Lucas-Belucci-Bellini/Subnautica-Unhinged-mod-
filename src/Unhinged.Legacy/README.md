# Unhinged.Legacy — ponte SMLHelper V2 → Nautilus

Mods de QModManager/SMLHelper não carregam no ramo moderno. Portá-los arquivo por arquivo
seria inviável: só o FCS tem **99.173 linhas** em 667 arquivos.

Mas a superfície legada é minúscula perto disso. Medido nas fontes reais:

| Símbolo SMLHelper | FCS | S.O.C.K. Tank |
| --- | ---: | ---: |
| `Buildable` | 282 | — |
| `Ingredient` | 163 | 10 |
| `TechData` | 130 | 4 |
| `ImageUtils` | 96 | — |
| `LanguageHandler` | 47 | 11 |
| `Spawnable` | 30 | — |
| `CraftDataHandler` | 26 | — |
| `ModUtils` | 17 | — |
| `AudioUtils` | 10 | 3 |
| `QModPatch` / `QModCore` | 8 / 8 | 1 / 1 |
| `OptionsPanelHandler` | 8 | 1 |
| `TechTypeHandler` | 6 | 1 |
| `Craftable` | 6 | 2 |
| resto (`ConsoleCommands`, `Sprite`, `KnownTech`, `CraftTree`, `PDA`, `BioReactor`) | ≤6 cada | — |

**~20 tipos cobrem os dois mods, somando mais de 106 mil linhas.** Daí a decisão: portar a
**API uma vez**, não os mods um a um.

## Como funciona

Este assembly **reimplementa os namespaces `SMLHelper.V2.*` e `QModManager.API.*`**,
encaminhando cada chamada para o Nautilus. A fonte legada compila sem alterar os `using`:
troca-se a referência de `SMLHelper.dll` por `Unhinged.Legacy.dll` e o resto segue igual.

Não há colisão com o SMLHelper legado instalado: o .NET identifica tipo por
**assembly + namespace + nome**, e o binding de compilação escolhe este.

O mapeamento segue o guia oficial
[`sml2-to-nautilus`](https://github.com/SubnauticaModding/Nautilus/blob/master/Nautilus/Documentation/guides/sml2-to-nautilus.md),
não dedução.

## Estado

Implementado: `Crafting` (`Ingredient`, `TechData`) e `Handlers.LanguageHandler`.
O resto entra por ordem de uso da tabela acima — cada tipo com a assinatura conferida
contra a assembly real antes de ser escrito.

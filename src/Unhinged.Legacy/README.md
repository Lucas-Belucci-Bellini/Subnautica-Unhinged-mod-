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

## Quebras do jogo moderno que este shim absorve

Descobertas lendo `Assembly-CSharp` do build atual — não são teoria, é o que impede o
código legado de compilar hoje:

**1. `Atlas.Sprite` não existe mais.** O namespace `Atlas` sumiu das duas assemblies do
jogo; o substituto é `UnityEngine.Sprite`. Só no FCS são **85 arquivos** com
`using Sprite = Atlas.Sprite;`. `Compat/AtlasSprite.cs` devolve o tipo, com conversão
implícita nos dois sentidos — os 85 arquivos não precisam ser tocados.

**2. `TechData` agora é um tipo estático do jogo, no namespace global.** Ele ganha a
resolução de nome contra o `SMLHelper.V2.Crafting.TechData` importado por `using`, então
qualquer uso não qualificado quebra com `CS0722`. O shim qualifica por extenso onde
precisa.

## A ponte herança → composição

É a diferença de fundo entre as duas APIs. O SMLHelper era **herança** (derive e
sobrescreva `GetGameObject`, `ClassID`, `GetBlueprintRecipe`…); o Nautilus é **composição**
(monte um `CustomPrefab` e pendure gadgets).

`ModPrefab` guarda um `CustomPrefab` por dentro e, no `Patch()`, liga os membros
sobrescritos aos gadgets equivalentes — `SetRecipe`, `SetPdaGroupCategory`, `SetUnlock`.
A classe derivada segue escrita como sempre foi.

A superfície necessária foi medida nas 20 classes do FCS que herdam dessas bases
(13 `Spawnable`, 6 `Buildable`, 1 `Craftable`): `TechType` (79 usos), `ClassID` (40),
`AssetsFolder` (39), `GetGameObject` (21), `GetItemSprite` (18), `OnFinishedPatching` (17),
`GroupForPDA`/`CategoryForPDA` (9 cada), `Patch` (8), `GetBlueprintRecipe` (8),
`OnStartedPatching` (7), `PrefabFileName`/`IconFileName`/`GetGameObjectAsync` (4 cada).
**17 membros** — todos implementados.

> ⚠️ A contagem bruta de `Buildable` (282) que aparecia numa versão anterior desta tabela
> estava inflada: quase tudo era `Buildables` (268, outro símbolo) e nomes de classes do
> próprio FCS. O que importa é a herança, e são 20 classes.

## Estado

| Área | Situação |
| --- | --- |
| `Crafting` (`Ingredient`, `TechData`) | ✅ |
| `Handlers.LanguageHandler` | ✅ |
| `Utility.ImageUtils` | ✅ |
| `Compat` (`Atlas.Sprite`) | ✅ |
| `Assets` (`ModPrefab`, `Spawnable`, `Craftable`, `Buildable`) | ✅ |
| `Handlers.CraftDataHandler` (`GetTechData`, `SetEquipmentType`, `SetQuickSlotType`) | ✅ |
| `Handlers.TechTypeHandler` (sobre o `EnumHandler` genérico) | ✅ |
| `Utility.AudioUtils` (+ `SoundChannel` → buses por jogo) | ✅ |
| `QModManager.API.ModLoading` + `LegacyModLoader` | ✅ |
| `Options` + `Commands` (atributos) | ⚠️ compila, **mas ainda não registra** em jogo |
| `Json.ConfigFile` | ✅ |
| `Utility.ModUtils` (save data — exige verificação própria) | pendente |
| `SpriteHandler`, `KnownTechHandler`, `CraftTreeHandler`, `PDAHandler` | pendente |

**Validado compilando de verdade** — e o resultado corrigiu duas conclusões minhas.
O `FCS_AlterraHub` (225 arquivos) compila contra esta ponte com **164 erros restantes**,
dos quais ~26 são handlers legados ainda ausentes aqui (`SpriteHandler`, `PDAHandler`,
`OptionsPanelHandler`, `CustomSoundHandler`, `PingHandler`, `SaveUtils`, `QModServices`) e
o resto é migração de API do jogo/Unity, que nenhum shim absorve.

Uma contagem menor que apareceu antes (**6**) era o compilador **abortando cedo** num
`CS0576`; e um grep por "SMLHelper" nos erros **não mede cobertura**, porque o compilador
reporta só o nome do tipo. Detalhe em [`docs/PORTE-LEGADO.md`](../../docs/PORTE-LEGADO.md).

Cada tipo é escrito **depois** de conferir a assinatura contra a assembly real. Nada é
deduzido de memória.

Procedimento de porte passo a passo: [`docs/PORTE-LEGADO.md`](../../docs/PORTE-LEGADO.md).

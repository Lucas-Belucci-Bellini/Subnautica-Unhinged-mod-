# Como portar um mod legado (QModManager/SMLHelper) para o jogo atual

Procedimento derivado do `Unhinged.Legacy`. A ideia é que o porte vire **mecânico**: a
ponte já traduz a API, então sobra trocar referências e corrigir o que o jogo mudou.

## 1. Trocar as referências no `.csproj`

| Sai | Entra |
| --- | --- |
| `SMLHelper.dll` | `Unhinged.Legacy.dll` |
| `QModManager.*.dll` | `Unhinged.Legacy.dll` (traz os atributos `[QMod*]`) |
| `Assembly-CSharp.dll` local | pacote `Subnautica.GameLibs` |
| — | `BepInEx.Core` 5.4.21 + `Nautilus.dll` |

Os `using SMLHelper.V2.*` **não mudam**: a ponte reexpõe os mesmos namespaces.

## 2. As três quebras do jogo moderno

Estas o compilador aponta, e são as únicas edições realmente necessárias na fonte:

### `Atlas.Sprite` não existe mais
O namespace `Atlas` sumiu das duas assemblies do jogo. **Já resolvido pela ponte** —
`Compat/AtlasSprite.cs` devolve o tipo com conversão implícita nos dois sentidos.
Nenhuma edição na fonte legada (só no FCS seriam 85 arquivos).

### ⚠️ `TechData` colide com um tipo estático do jogo
O jogo moderno tem um `TechData` **estático no namespace global**. Pela regra de resolução
de nomes do C#, membros do namespace ganham dos `using` em escopo de arquivo — então
`TechData` resolve para o do jogo e o build morre com **`CS0722`/`CS0721`**.

A ponte **não consegue** resolver isto por você: é resolução de nome na fonte legada.
A correção é uma linha por arquivo afetado:

```csharp
using TechData = SMLHelper.V2.Crafting.TechData;
```

Aplicável em lote nos arquivos que o compilador apontar.

### `PlaySound` mudou de forma
Virou `TryPlaySound`, com o canal expresso por caminho de bus do FMOD. A ponte mantém
`PlaySound(sound, SoundChannel)` e traduz. **Importante:** os caminhos de bus **diferem
entre Subnautica e Below Zero** — a ponte referencia
`Nautilus.Utility.AudioUtils.BusPaths`, que é `partial` por jogo. Copiar a string em vez
de referenciar a constante dá som mudo, em silêncio.

## 3. Ponto de entrada

`[QModCore]` e `[QModPatch]` só **marcam** código — quem os executava era o QModManager,
que não existe aqui. A ponte fornece os atributos (para compilar) e o executor:

```csharp
[BepInPlugin("com.exemplo.modlegado", "Mod Legado", "1.0.0")]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    private void Awake() => LegacyModLoader.Run(typeof(ClasseDoMod).Assembly, Logger);
}
```

`LegacyModLoader` respeita a ordem original (pré-patch → patch → pós-patch), desembrulha
`TargetInvocationException` para o log ficar legível, e **isola falhas**: um mod quebrado
não derruba os outros.

## 3.5 Resultado medido: Alterra Hub compilado contra a ponte

Compilado de verdade — 225 arquivos do `FCS_AlterraHub` mais o shared project `FCSCommon`,
contra `Unhinged.Legacy` + `Subnautica.GameLibs` + Nautilus 1.0.0-pre.53.

### ⚠️ Duas armadilhas de medição, ambas encontradas na prática

**1. Case-sensitivity engana.** O `.csproj` diz `Mono\`, o disco tem `mono/`. No Windows
dá na mesma; no Linux, 27 arquivos "somem" e parece que a fonte publicada está incompleta.
**Não está** — os 225 resolvem, com comparação case-insensitive. Uma conclusão errada aqui
teria mandado o porte reimplementar um subsistema inteiro que já existe.

**2. Contagem de erro baixa pode ser o compilador desistindo.** Com um `CS0576` no
`Mod.cs`, o total parecia **6**. Corrigido esse erro, o compilador foi adiante e o número
real apareceu: **164**. Erro fatal cedo mascara o resto — comemorar contagem baixa sem
antes eliminar os fatais é enganar a si mesmo.

**3. Grepar pelo namespace não mede cobertura da ponte.** Nenhum dos 164 erros escreve
"SMLHelper", mas vários nomes ausentes — `SpriteHandler`, `PDAHandler`,
`OptionsPanelHandler`, `CustomSoundHandler`, `PingHandler`, `SaveUtils`, `QModServices` —
**são** API legada que falta na ponte. O erro diz só o nome do tipo, não de onde ele vinha.

### O estado real

| Categoria | Erros | Natureza |
| --- | ---: | --- |
| Handlers legados ainda ausentes na ponte | ~26 | `SpriteHandler`, `PDAHandler`, `OptionsPanelHandler`, `CustomSoundHandler`, `PingHandler`, `SaveUtils`, `IIngredient`, `QModServices` |
| API do jogo mudou | ~90 | `CraftData.GetItemSize`, `CraftData.techData`, `CraftData.cookedCreatureList` e outros membros removidos |
| Unity/UI mudou | ~10 | `Text` → `TMPro.TextMeshProUGUI` |
| Spawn passou para o Nautilus | ~16 | `CoordinatedSpawnsHandler`, `SpawnInfo` — existem no Nautilus, precisam do `using` certo |
| Resto | ~22 | construtores, acessibilidade, conversões |

➡️ **A ponte encurtou o trabalho, não o eliminou.** As bases de asset, crafting, idioma,
áudio e o carregador funcionam; falta uma segunda leva de handlers, e há migração de API do
jogo que **nenhum shim absorve** — é edição de fonte, arquivo a arquivo.

### Regra de porte que este teste revelou

**Onde o FCS tem `#if BELOWZERO`, o Subnautica moderno costuma precisar do ramo do Below
Zero.** O `ITooltip` é o caso exemplar: o FCS já implementava `showTooltipOnDrag` e
`GetTooltip(TooltipData)`, mas só sob `#if BELOWZERO` — porque o Subnautica de então tinha
a API antiga. Os dois jogos convergiram. Antes de escrever implementação nova, **procure se
o ramo BZ já a tem.**

### Correção à orientação do §2

O alias do `TechData` **precisa ficar dentro do `namespace`**, não no topo do arquivo:

```csharp
namespace MeuMod
{
    using TechData = SMLHelper.V2.Crafting.TechData;   // ✅ aqui
```

Em escopo de arquivo ele perde para o tipo global do jogo e dá **`CS0576`** — verificado
compilando, depois de eu ter documentado errado a primeira vez.

### Correções que o teste provocou na ponte

`GetItemSprite` tinha de ser `protected` (16 classes do FCS o sobrescrevem assim);
faltavam `UnlockedAtStart`, `EntityInfo` (`UWE.WorldEntityInfo`) e `DiscoverMessage` nas
bases; e faltava a propriedade **`Order`** nos atributos de opções — usada pelo FCS para
ordenar o painel, e invisível em qualquer leitura que não fosse compilar.

## 3.55 Truque que evitou 46 edições: extensão em namespace global

O `HandReticle` sozinho era **48 dos 152 erros**. A saída não foi editar 48 lugares na
fonte do FCS — foi devolver a API antiga como **métodos de extensão declarados sem
namespace**.

Funciona por duas razões que se somam:

1. **O membro de instância não existe mais**, então o compilador aceita a extensão. (Se
   ainda existisse, o método de instância venceria e a extensão seria ignorada.)
2. **Sem `namespace`, a extensão vale em todo arquivo**, sem `using`. Isso importa porque
   não há como saber quais `using` cada arquivo legado tem — e o objetivo é não tocar neles.

```csharp
// sem namespace, de propósito
public static class HandReticleLegacyExtensions
{
    public static void SetInteractText(this HandReticle r, string key)
        => r.SetText(HandReticle.TextType.Hand, key, true, GameInput.Button.None);
}
```

**Resultado: 152 → 106 erros, com zero edição na fonte de terceiro.**

⚠️ **Onde o truque NÃO alcança**, e é bom saber a fronteira:

- **Tipo de argumento que sumiu.** 5 chamadas passam `HandReticle.Hand.None`; o tipo `Hand`
  não existe mais, e o erro acontece no argumento, **antes** da resolução de sobrecarga.
  Extensão não ajuda — é edição na fonte (`GameInput.Button.None`).
- **Membro estático removido de classe estática.** `CraftData.GetItemSize` migrou para
  `TechData.GetItemSize` (mesma assinatura, devolve `Vector2int`). Não dá para estender uma
  chamada estática: é `sed` de `CraftData.GetItemSize` → `TechData.GetItemSize`.

## 3.6 Estado atual: a ponte terminou; falta migrar a API do jogo

Depois da segunda leva de handlers (`SpriteHandler`, `PDAHandler`, `CustomSoundHandler`,
`PingHandler`, `SaveUtils`, `OptionsPanelHandler`, `QModServices`), o Alterra Hub está em
**152 erros — e nenhum deles é da ponte.** Todos são API do jogo que mudou:

| Tipo do jogo | Erros | Migração |
| --- | ---: | --- |
| ~~`HandReticle`~~ | ~~48~~ → **0** | ✅ **resolvido pela ponte** (extensão em namespace global — ver §3.55) |
| `EndCreditsManager` | ~20 | créditos finais reescritos; campos (`centerText`, `leftText`, `goToPos`…) não existem mais |
| **`CraftData.GetItemSize`** | 10 | → **`TechData.GetItemSize(TechType)`**, mesma assinatura (`Vector2int`). Chamada estática: exige `sed` na fonte, a ponte não alcança |
| `PDA.screen`, `Player.pdaSpawn` | 10 | campos removidos |
| `PDAEncyclopedia.EntryData.timeCapsule` | 4 | campo removido |
| `InputField.SetText` | 2 | Unity UI |
| resto | ~58 | construtores, acessibilidade, conversões, `Text`→`TextMeshProUGUI` |

➡️ **`HandReticle` sozinho é um terço.** Migrá-lo é o passo de maior retorno, e é mecânico:
três métodos viraram dois, com o destino do texto passando a ser um parâmetro `TextType`.

## 4. O que a ponte ainda não cobre

| Pendente | Por quê |
| --- | --- |
| `ModUtils.Save` / `LoadSaveData` | Mexe em **dados de save**. Errar aqui corrompe o save do jogador, então merece verificação própria antes de ser escrito — não vale deduzir. |
| `OptionsPanelHandler` / `ConsoleCommandsHandler` | ⚠️ Os **atributos** existem e fazem compilar, mas são **apenas de declaração**: o painel de opções e os comandos de console **ainda não são registrados em jogo**. Um mod portado agora terá as opções ignoradas — sem erro visível. Ligar ao `Nautilus.Options` e ao `ConsoleCommandsHandler` é o próximo passo. |
| `SpriteHandler`, `KnownTechHandler`, `CraftTreeHandler`, `PDAHandler`, `BioReactorHandler` | ≤6 usos cada. |

## 5. Ordem recomendada

1. **Alterra Hub** (FCS, MIT) — é dependência de todos os outros módulos FCS.
2. Um módulo FCS pequeno, para validar prefabs, receitas, PDA e assets de ponta a ponta.
3. Os demais módulos FCS, um a um.
4. **S.O.C.K. Tank** — usa um subconjunto do que o FCS já exige, então vem quase de graça
   *do ponto de vista técnico*. Continua bloqueado por licença: ver
   [`PORTE-SOCK-TANK.md`](PORTE-SOCK-TANK.md).

Cada módulo é testado **isolado** antes de entrar no pacote integrado, conforme o `PLAN.md`.

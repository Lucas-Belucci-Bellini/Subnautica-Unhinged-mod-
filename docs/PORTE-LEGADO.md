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

## 4. O que a ponte ainda não cobre

| Pendente | Por quê |
| --- | --- |
| `ModUtils.Save` / `LoadSaveData` | Mexe em **dados de save**. Errar aqui corrompe o save do jogador, então merece verificação própria antes de ser escrito — não vale deduzir. |
| `OptionsPanelHandler` | 8 usos no FCS; mapeia para `Nautilus.Options`, ainda não conferido. |
| `ConsoleCommandsHandler` | 6 usos. |
| `SpriteHandler`, `KnownTechHandler`, `CraftTreeHandler`, `PDAHandler`, `BioReactorHandler` | ≤6 usos cada. |

## 5. Ordem recomendada

1. **Alterra Hub** (FCS, MIT) — é dependência de todos os outros módulos FCS.
2. Um módulo FCS pequeno, para validar prefabs, receitas, PDA e assets de ponta a ponta.
3. Os demais módulos FCS, um a um.
4. **S.O.C.K. Tank** — usa um subconjunto do que o FCS já exige, então vem quase de graça
   *do ponto de vista técnico*. Continua bloqueado por licença: ver
   [`PORTE-SOCK-TANK.md`](PORTE-SOCK-TANK.md).

Cada módulo é testado **isolado** antes de entrar no pacote integrado, conforme o `PLAN.md`.
